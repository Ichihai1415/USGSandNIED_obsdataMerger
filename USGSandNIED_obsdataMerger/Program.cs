using Newtonsoft.Json.Linq;

namespace USGSandNIED_obsdataMerger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-----NIED START-----");
            Dictionary<string, string> niedData = File.ReadLines("D:\\Ichihai1415\\data\\csv\\kyoshin.20240101noto.shindo.csv")
                .Where(x => x.Contains('.')).Select(x => x.Split(',')).ToDictionary(d => d[0], d => d[1]);
            foreach (var data in niedData)
                Console.WriteLine($"{data.Key} {data.Value}");
            Console.WriteLine("----- NIED END -----");
            Console.WriteLine("-----USGS START-----");
            Dictionary<string, string> usgsData = JObject.Parse(File.ReadAllText("D:\\Ichihai1415\\data\\json\\usgs\\noto2024-stationlist.json")).SelectToken("features")
                .Where(d => !((string)d.SelectToken("properties.code")).StartsWith("UTM")).Where(d => (string)d.SelectToken("properties.intensity") != "null")
                .ToDictionary(d => (string)d.SelectToken("properties.code"), d => (string)d.SelectToken("properties.intensity"));
            foreach (var data in usgsData)
                Console.WriteLine($"{data.Key} {data.Value}");
            Console.WriteLine("----- USGS END -----");
            Console.WriteLine("----- ALL START-----");
            string NIEDUSGSdata = "観測点コード,NIED計測震度,USGSM改正メルカリ震度階級\n" +
               string.Join("\n", niedData.Keys.Intersect(usgsData.Keys).Select(key => $"{key},{niedData[key]},{usgsData[key]}"));
            Console.WriteLine(NIEDUSGSdata);
            Console.WriteLine("-----  ALL END -----");
            File.WriteAllText(DateTime.Now.ToString("yyyyMMddHHmmss") + ".output.csv", NIEDUSGSdata);
        }
    }
}
