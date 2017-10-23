using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkGreenODBCUtility
{
    class Batch
    {
        public static void Exec(string cmd)
        {
            string tempFilename = Path.ChangeExtension(Path.GetTempFileName(), ".bat");
            using (StreamWriter writer = new StreamWriter(tempFilename))
            {
                writer.WriteLine(cmd);
            }
            Process process = Process.Start(tempFilename);
            process.WaitForExit();
            File.Delete(tempFilename);
        }
    }
}
