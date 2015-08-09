using System;
using System.Collections.Generic;
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
        /// output profiling result to file
        /// </summary>
        /// <param name="outputFile"></param>
        public void OutputResultToFile(string outputFile)
        {
            prResult.WriteResult(outputFile);
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
                    catch (Exception)
                    {
                        // comes when Thread.Suspend() fails.
                    }
                    finally
                    {
                        // resume target thread
                        try
                        {
                            targetThread.Resume();
                        }
                        catch
                        {
                            // do nothing
                        }
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
                ProfileResultItem findt = find.SearchChildren(bsMethod.ToString(), class_name);

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
        /// write profiling result to file
        /// </summary>
        /// <param name="filename"></param>
        public void WriteResult(string filename)
        {
            XmlSerializer ser = new XmlSerializer(typeof(List<ProfileResultItem>));

            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                ser.Serialize(fs, resultList);
            }
        }
    }

    [Serializable]
    /// <summary>
    /// function information
    /// </summary>
    public class ProfileResultItem
    {
        /// <summary>
        /// children
        /// </summary>
        private List<ProfileResultItem> children = new List<ProfileResultItem>();

        /// <summary>
        /// parent
        /// </summary>
        private ProfileResultItem parent;


        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// short function name
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// function name for display
        /// </summary>
        public string FunctionString { get; set; }

        /// <summary>
        /// class name
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// parent name
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// parent id
        /// </summary>
        public int ParentID { get; set; }

        /// <summary>
        /// depth of this item
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// called count
        /// </summary>
        public int ProfileCount { get; set; }


        /// <summary>
        /// constructor
        /// </summary>
        public ProfileResultItem() { }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parent"></param>
        /// <param name="function_name"></param>
        /// <param name="function_string"></param>
        /// <param name="class_name"></param>
        public ProfileResultItem(
            int id,
            ProfileResultItem parent,
            string function_name,
            string function_string,
            string class_name)
        {
            this.ID = id;
            this.parent = parent;
            this.FunctionName = function_name;
            this.FunctionString = function_string;
            this.ClassName = class_name;

            if (parent != null)
            {
                this.Depth = parent.Depth + 1;
                this.ParentName = parent.FunctionName;
                this.ParentID = parent.ID;
            }
            else
            {
                this.Depth = 0;
                this.ParentName = "";
                this.ParentID = -1;
            }

            this.ProfileCount = 0;
        }

        /// <summary>
        /// add child
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(ProfileResultItem child)
        {
            children.Add(child);
        }

        /// <summary>
        /// increment
        /// </summary>
        public void Increment()
        {
            ProfileCount++;
        }

        /// <summary>
        /// search function from children
        /// </summary>
        /// <param name="func_nm"></param>
        /// <param name="class_nm"></param>
        /// <returns></returns>
        public ProfileResultItem SearchChildren(string func_nm, string class_nm)
        {
            // input check
            if (func_nm == null || class_nm == null) return null;

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].FunctionString.Equals(func_nm) && children[i].ClassName.Equals(class_nm)) return children[i];
            }

            return null;
        }
    }
}
