using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiS3ACLSetting
{
    class Helper
    {
        public static List<Media> GetListOfMediaObjects(Param param)
        {
            if (File.Exists(param.Filename))
                return GetList<Media>(param.Filename, param.FileDelimiter, param.FileRowNumber, param.FileExclude);
            else
                return null;
        }

        private static List<T> GetList<T>(string filename, char delimiter, int rownumber, string exclude) where T : IMedia, new()
        {
            if (File.Exists(filename))
            {
                var list = new List<T>();

                FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    string Line = streamReader.ReadLine();

                    exclude = (!String.IsNullOrWhiteSpace(exclude)) ? exclude : null;
                    while (!String.IsNullOrEmpty(Line))
                    {
                        string[] columns = Line.Split(delimiter);

                        var row = columns[rownumber];

                        if (!String.IsNullOrWhiteSpace(row))
                        {
                            if(exclude != null)
                            {
                                if (row.IndexOf(exclude) < 0 )
                                {
                                    list.Add(new T { key = row });
                                }
                            } 
                            else
                            {
                                list.Add(new T { key = row });
                            }                            
                        }

                        Line = streamReader.ReadLine();
                    }
                }
                return list;
            } 
            else {
                return null;
            }                
        }
    }
}
