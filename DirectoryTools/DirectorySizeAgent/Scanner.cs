using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectorySizeAgent
{
    public class Scanner
    {
        [Pure]
        public static long GetDirectorySize(string path)
        {
            try
            {
                var di = new DirectoryInfo(path);

                return FileList(di, true, null).Where(f=>f!=null).Sum(f=>f.Length);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Error in GetDirectorySize({0}). Exception:{1}", path, ex));
                return 0;
            }
        }

        [Pure]
        public static List<DirectoryInfo> DirectoryList(DirectoryInfo di, bool searchAllDirectories, Predicate<string> filter)
        {
            List<DirectoryInfo> dirs = new List<DirectoryInfo>();

            try
            {
                Parallel.ForEach(di.GetDirectories(), folder =>
                {
                    try
                    {
                        if ((filter == null) || (filter(folder.FullName)))
                        {
                            dirs.Add(folder);

                            if (searchAllDirectories)
                                dirs.AddRange(DirectoryList(folder, true,
                                    filter));
                        }
                    }

                    catch (UnauthorizedAccessException)
                    {
                        // don't really need to do anything 
                        // user just doesn't have access 
                    }

                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Error in SubTree for folder ({0}). Exception:{1}", folder.Name, ex));
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Error in Directory list for folder ({0}). Exception:{1}", di.Name, ex));
            }

            return dirs;
        }

        [Pure]
        public static List<FileInfo> FileList(DirectoryInfo root, bool searchAllDirectories, Predicate<string> filter)
        {
            List<FileInfo> files = new List<FileInfo>();

            try
            {
                List<DirectoryInfo> DirList = new List<DirectoryInfo> { root };

                if (searchAllDirectories)
                    DirList.AddRange(DirectoryList(root, true, null));

                Parallel.ForEach(DirList, di =>
                {
                    try
                    {
                        if (di != null)
                        {
                            foreach (FileInfo file in di.GetFiles())
                            {
                                try
                                {
                                    if ((filter == null) || (filter(file.FullName)))
                                        files.Add(file);
                                }

                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(string.Format("Error in file ({0}). Exception:{1}", file.Name, ex));
                                }
                            }
                        }
                    }

                    catch (UnauthorizedAccessException)
                    {
                        // don't really need to do anything 
                        // user just doesn't have access 
                    }

                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Error in getting files for dir ({0}). Exception:{1}", di.Name, ex));
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Error in getting files for dir ({0}). Exception:{1}", root.Name, ex));
            }

            return files;
        } 
    }
}
