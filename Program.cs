using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

public class Figure
{
    public string Name { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public Figure()
    {

    }

    public Figure(string name, double width, double height)
    {
        Name = name;
        Width = width;
        Height = height;
    }
}

public class FileManager
{
    private List<Figure> figures;

    public FileManager()
    {
        figures = new List<Figure>();
    }

    public List<Figure> Figures
    {
        get { return figures; }
    }

    public void LoadFromFile(string filePath)
    {
        string fileExtension = Path.GetExtension(filePath);

        switch (fileExtension)
        {
            case ".txt":
                LoadFromTxt(filePath);
                break;
            case ".json":
                LoadFromJson(filePath);
                break;
            case ".xml":
                LoadFromXml(filePath);
                break;
            default:
                Console.WriteLine("Неподдерживаемый формат файла.");
                break;
        }
    }

    private void LoadFromTxt(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);

        string name = null;
        double width = 0;
        double height = 0;

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (name == null)
            {
                name = line;
            }
            else if (width == 0)
            {
                if (double.TryParse(line, out width))
                {
                    width = double.Parse(line);
                }
                else
                {
                    Console.WriteLine("Ошибка в формате данных в файле TXT.");
                    return;
                }
            }
            else if (height == 0)
            {
                if (double.TryParse(line, out height))
                {
                    height = double.Parse(line);
                    figures.Add(new Figure(name, width, height));
                    name = null;
                    width = 0;
                    height = 0;
                }
                else
                {
                    Console.WriteLine("Ошибка в формате данных в файле TXT.");
                    return;
                }
            }
        }

        if (figures.Count > 0)
        {
            Console.WriteLine("Данные загружены из файла в формате TXT:");
            foreach (var figure in figures)
            {
                Console.WriteLine();
                Console.WriteLine($"{figure.Name}\n{figure.Width}\n{figure.Height}");
            }
        }
        else
        {
            Console.WriteLine("Недостаточно данных в файле TXT.");
        }
    }

    private void LoadFromJson(string filePath)
    {
        string json = File.ReadAllText(filePath);
        figures = JsonConvert.DeserializeObject<List<Figure>>(json);

        if (figures != null && figures.Count > 0)
        {
            Console.Clear();
            Console.WriteLine("Данные из JSON файла:");
            Console.WriteLine();

            foreach (var figure in figures)
            {
                Console.WriteLine($"Название фигуры: {figure.Name}");
                Console.WriteLine($"Ширина: {figure.Width}");
                Console.WriteLine($"Высота: {figure.Height}");
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("Нет данных в файле JSON.");
        }
    }

    private void LoadFromXml(string filePath)
    {
        using (XmlReader reader = XmlReader.Create(filePath))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Figure>));
            var loadedFigures = (List<Figure>)serializer.Deserialize(reader);

            if (loadedFigures != null && loadedFigures.Count > 0)
            {
                figures.AddRange(loadedFigures);
                Console.Clear();
                Console.WriteLine("Данные загружены из файла в формате XML:");
                Console.WriteLine();
                foreach (var figure in loadedFigures)
                {
                    Console.WriteLine($"Название фигуры: {figure.Name}\n Ширина: {figure.Width}\n Высота: {figure.Height}");
                }
            }
            else
            {
                Console.WriteLine("Нет данных в файле XML.");
            }
        }
    }

    public void SaveToFile(string filePath, List<Figure> figures)
    {
        string fileExtension = Path.GetExtension(filePath);

        switch (fileExtension)
        {
            case ".txt":
                SaveToTxt(filePath, figures);
                break;
            case ".json":
                SaveToJson(filePath, figures);
                break;
            case ".xml":
                SaveToXml(filePath, figures);
                break;
            default:
                Console.WriteLine("Неподдерживаемый формат файла для сохранения.");
                break;
        }
    }

    private void SaveToTxt(string filePath, List<Figure> figures)
    {
        if (figures != null && figures.Count > 0)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var figure in figures)
                {
                    writer.WriteLine(figure.Name);
                    writer.WriteLine(figure.Width);
                    writer.WriteLine(figure.Height);
                }
            }

            Console.WriteLine("Данные сохранены в файл в формате TXT.");
        }
        else
        {
            Console.WriteLine("Нет данных для сохранения.");
        }
    }

    private void SaveToJson(string filePath, List<Figure> figures)
    {
        if (figures != null && figures.Count > 0)
        {
            string json = JsonConvert.SerializeObject(figures, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filePath, json);
            Console.WriteLine("Данные сохранены в файл в формате JSON:");
        }
        else
        {
            Console.WriteLine("Нет данных для сохранения.");
        }
    }

    private void SaveToXml(string filePath, List<Figure> figures)
    {
        if (figures != null && figures.Count > 0)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "    "
            };

            using (XmlWriter writer = XmlWriter.Create(filePath, settings))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Figure>));
                serializer.Serialize(writer, figures);
            }
            Console.WriteLine("Данные сохранены в файл в формате XML.");
        }
        else
        {
            Console.WriteLine("Нет данных для сохранения.");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        List<Figure> figures = new List<Figure>();

        while (true)
        {
            Console.Clear();
            Console.Write("Введите путь до файла (вместе с названием), который вы хотите открыть: \n----------------------------------------------------------------------\n");
            string filePath = Console.ReadLine();

            FileManager fileManager = new FileManager();
            fileManager.LoadFromFile(filePath);

            if (fileManager.Figures != null && fileManager.Figures.Count > 0)
            {
                figures.AddRange(fileManager.Figures);
            }

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("F1 для сохранения в JSON, XML, TXT\nENTER для открытия нового файла\nESC для выхода из программы");

                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.F1)
                {
                    Console.Clear();
                    Console.Write("Введите путь до файла (вместе с названием), куда вы хотите сохранить текст:\n");
                    string savePath = Console.ReadLine();
                    fileManager.SaveToFile(savePath, figures);
                }
                else if (key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (key == ConsoleKey.Escape)
                {
                    return;
                }
            }
        }
    }
}