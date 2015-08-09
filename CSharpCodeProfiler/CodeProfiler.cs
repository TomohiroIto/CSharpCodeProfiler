using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace CSharpCodeProfiler
{
#pragma warning disable 618

    public class CodeProfiler : IDisposable
    {
        /// <summary>
        /// Profiling target thread
        /// </summary>
        private Thread targetThread = null;
        /// <summary>
        /// Profiler thread
        /// </summary>
        private Thread profilerThread = null;
        /// <summary>
        /// Profiling result
        /// </summary>
        private ProfileResult prResult = null;


        /// <summary>
        /// Constructor
        /// </summary>
        public CodeProfiler()
        {
            // target thread
            targetThread = Thread.CurrentThread;

            // result object
            prResult = new ProfileResult(new StackTrace(targetThread, false).GetFrames());

            // start profiler thread
            profilerThread = new Thread(this.Run);
            profilerThread.Priority = ThreadPriority.AboveNormal;
            profilerThread.SetApartmentState(ApartmentState.STA);
            profilerThread.IsBackground = true;
            profilerThread.Start();
        }

        /// <summary>
        /// Stop profiler thread
        /// </summary>
        /// <returns></returns>
        public void Stop()
        {
            if (profilerThread != null && profilerThread.IsAlive)
            {
                try
                {
                    // thread abort
                    profilerThread.Abort();
                    profilerThread.Join();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Output result as a xml file
        /// </summary>
        /// <returns></returns>
        public void OutputResult(string outputFile)
        {
            prResult.GetResultTable().WriteXml(outputFile);
        }

        /// <summary>
        /// Main logic
        /// </summary>
        private void Run()
        {
            try
            {
                while (true)
                {
                    // wait 1 ms
                    Thread.Sleep(1);

                    try
                    {
                        // check target thread
                        if (targetThread == null || !targetThread.IsAlive) break;

                        // suspend target thread
                        targetThread.Suspend();

                        // stach check
                        stackWalk();
                    }
                    catch (ThreadAbortException ex)
                    {
                        // accept thread abort
                        throw ex;
                    }
                    finally
                    {
                        // resume target thread
                        targetThread.Resume();
                    }
                }
            }
            catch (Exception)
            {
                // from ThreadAbortException
            }
        }

        /// <summary>
        /// save stack information
        /// </summary>
        private void stackWalk()
        {
            // get stack information
            StackTrace stackTrace = new StackTrace(targetThread, false);
            StackFrame[] fms = stackTrace.GetFrames();

            // save to result object
            prResult.Add(fms);
        }

        /// <summary>
        /// IDisposable.Dispose
        /// </summary>
        public void Dispose()
        {
            Stop();
        }
    }
#pragma warning restore 618

    [Serializable]
    /// <summary>
    /// Profiling result class
    /// </summary>
    public class ProfileResult
    {
        /// <summary>
        /// List of each profile item
        /// </summary>
        private List<ProfileResultItem> resultList = new List<ProfileResultItem>();
        /// <summary>
        /// result (tree)
        /// </summary>
        private ProfileResultItem result = null;
        /// <summary>
        /// StackFrame length at the start
        /// </summary>
        private int startLength;
        /// <summary>
        /// profiling count
        /// </summary>
        private int profileCount = 0;
        /// <summary>
        /// for item id
        /// </summary>
        private int item_id = 0;


        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="sfStart"></param>
        public ProfileResult(StackFrame[] sfStart)
        {
            result = new ProfileResultItem(item_id++, null, "<TOP>", "", "");
            startLength = sfStart.Length;
        }

        /// <summary>
        /// add profiling result
        /// </summary>
        /// <param name="fms"></param>
        public void Add(StackFrame[] fms)
        {
            // count increment
            profileCount++;
            result.Increment();

            // skip if thread is in the top function
            if (fms.Length <= startLength)
            {
                return;
            }

            // search algorithm
            ProfileResultItem find = result;
            for (int i = fms.Length - startLength; i >= 0; i--)
            {
                // function
                MethodBase bsMethod = fms[i].GetMethod();

                // search next
                string class_name = bsMethod.ReflectedType == null ? "" : bsMethod.ReflectedType.FullName;
                ProfileResultItem findt = find.Find(bsMethod.ToString(), class_name);

                // create if not found
                if (findt == null)
                {
                    findt = new ProfileResultItem(item_id++, find, bsMethod.Name, bsMethod.ToString(), class_name);

                    // add to the list
                    resultList.Add(findt);

                    // add to paremt
                    find.AddChild(findt);
                }

                // switch the current parent
                find = findt;

                // increment
                find.Increment();
            }
        }

        /// <summary>
        /// get the result as a DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable GetResultTable()
        {
            // create table
            DataTable resultTable = new DataTable();
            resultTable.Namespace = "";
            resultTable.TableName = "PROFILE";

            // configure columns
            resultTable.Columns.Add("ID", typeof(int));
            resultTable.Columns.Add("FUNCTION_NAME", typeof(string));
            resultTable.Columns.Add("FUNCTION_STRING", typeof(string));
            resultTable.Columns.Add("CLASS_NAME", typeof(string));
            resultTable.Columns.Add("PARENT_NAME", typeof(string));
            resultTable.Columns.Add("PARENT_ID", typeof(int));
            resultTable.Columns.Add("DEPTH", typeof(int));
            resultTable.Columns.Add("PROFILE", typeof(int));

            // copy data
            foreach (ProfileResultItem item in resultList)
            {
                DataRow row = resultTable.NewRow();
                row["ID"] = item.ID;
                row["FUNCTION_NAME"] = item.FunctionName;
                row["FUNCTION_STRING"] = item.FunctionString;
                row["CLASS_NAME"] = item.ClassName;
                row["PARENT_NAME"] = item.ParentName;
                row["PARENT_ID"] = item.ParentID;
                row["DEPTH"] = item.Ladder;
                row["PROFILE"] = item.ProfileCount;
                resultTable.Rows.Add(row);
            }

            return resultTable;
        }
    }

    [Serializable]
    /// <summary>
    /// function information
    /// </summary>
    public class ProfileResultItem : IComparable<ProfileResultItem>
    {
        /// <summary>
        /// children
        /// </summary>
        private List<ProfileResultItem> children = new List<ProfileResultItem>();
        /// <summary>
        /// sort flag
        /// </summary>
        private bool sorted = false;


        /// <summary>
        /// ID
        /// </summary>
        private int id = 0;
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// short function name
        /// </summary>
        private string function_name;
        public string FunctionName
        {
            get { return function_name; }
            set { function_name = value; }
        }

        /// <summary>
        /// function name for display
        /// </summary>
        private string function_string;
        public string FunctionString
        {
            get { return function_string; }
            set { function_string = value; }
        }

        /// <summary>
        /// class name
        /// </summary>
        private string class_name;
        public string ClassName
        {
            get { return class_name; }
            set { class_name = value; }
        }

        /// <summary>
        /// parent
        /// </summary>
        private ProfileResultItem parent;
        public string ParentName
        {
            get { return (parent == null) ? "" : parent.function_name; }
            set { }
        }
        public int ParentID
        {
            get { return (parent == null) ? -1 : parent.id; }
            set { }
        }

        /// <summary>
        /// depth of this item
        /// </summary>
        private int ladder;
        public int Ladder
        {
            get { return ladder; }
            set { ladder = value; }
        }

        /// <summary>
        /// called count
        /// </summary>
        private int profile_count;
        public int ProfileCount
        {
            get { return profile_count; }
            set { profile_count = value; }
        }


        /// <summary>
        /// constructor
        /// </summary>
        public ProfileResultItem() { }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="function_name"></param>
        /// <param name="function_string"></param>
        public ProfileResultItem(
            int id,
            ProfileResultItem parent,
            string function_name,
            string function_string,
            string class_name)
        {
            this.id = id;
            this.parent = parent;
            this.function_name = function_name;
            this.function_string = function_string;
            this.class_name = class_name;

            if (parent != null)
                this.ladder = parent.ladder + 1;
            else this.ladder = 0;

            this.profile_count = 0;
        }

        /// <summary>
        /// add child
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(ProfileResultItem child)
        {
            children.Add(child);
            sorted = false;
        }

        /// <summary>
        /// increment
        /// </summary>
        public void Increment()
        {
            profile_count++;
        }

        /// <summary>
        /// search function from children
        /// </summary>
        /// <param name="func_nm"></param>
        /// <param name="class_nm"></param>
        /// <returns></returns>
        public ProfileResultItem Find(string func_nm, string class_nm)
        {
            // input check
            if (func_nm == null || class_nm == null) return null;

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].function_string.Equals(func_nm) && children[i].class_name.Equals(class_nm)) return children[i];
            }

            return null;
        }

        /// <summary>
        /// sort (currently not used)
        /// </summary>
        private void sort()
        {
            if (sorted) return;

            children.Sort();
            sorted = true;
        }

        /// <summary>
        /// IComparable<ProfileResultItem>.CompareTo
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ProfileResultItem other)
        {
            int c = this.profile_count.CompareTo(other.profile_count);
            if (c != 0) return c;

            return this.ladder.CompareTo(other.ladder);
        }
    }
}
