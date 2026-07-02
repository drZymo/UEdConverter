using System.Globalization;
using System.IO;
using UedConverter.Converter;
using UedConverter.UtxFile;

namespace UedConverter;

public class Progra
{
    public static int Main(string[] args)
    {
        if (args.Length < 2 || args.Length > 3)
        {
            Console.WriteLine("Usage: UedConverter <input_file> <output_file>");
            return 0;
        }

        var inputFile = args[0];
        var outputFile = args[1];

        bool loadTextureData = args.Length == 3 && args[2] == "-t";

        IUedConverter converter;
        if (Path.GetExtension(inputFile).ToLower(CultureInfo.InvariantCulture) == ".obj" &&
            Path.GetExtension(outputFile).ToLower(CultureInfo.InvariantCulture) == ".t3d")
        {
            converter = new Converter.O2U_Converter();
        }
        else if (Path.GetExtension(inputFile).ToLower(CultureInfo.InvariantCulture) == ".t3d" &&
            Path.GetExtension(outputFile).ToLower(CultureInfo.InvariantCulture) == ".obj")
        {
            converter = new Converter.U2O_Converter(loadTextureData);
        }
        else
        {
            Console.Error.WriteLine("Unsupported file type conversion.");
            return 1;
        }

        var inputLines = File.ReadAllLines(inputFile);

        var outputLines = converter.Convert(inputLines);

        if (converter is U2O_Converter u2o)
        {
            if (u2o.MissingTextureData.Count > 0)
            {
                File.WriteAllLines("UedConverterLog.txt", u2o.MissingTextureData);
                Console.Error.WriteLine($"Not all textures could be found in data file (total: {u2o.MissingTextureData.Count}).");
                Console.Error.WriteLine($"Examples:\n{string.Join("\n", u2o.MissingTextureData.Take(4))}");
                Console.Error.WriteLine($"See UedConverterLog.txt for more information.");
            }
        }

        File.WriteAllLines(outputFile, outputLines);

        return 0;
    }
}
