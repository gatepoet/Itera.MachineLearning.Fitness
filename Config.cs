using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itera.MachineLearning.Fitness
{
    public class Config
    {
        public static readonly string BasePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "MachineLearning");
    }
}
