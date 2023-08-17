﻿using System;
using System.IO;
using Newtonsoft.Json;

namespace PointsToTriangles;

public static class JsonExt
{
    public static string ToJson(this object obj) => JsonConvert.SerializeObject(obj);

    public static string ToJson(this   object          obj,
                                params JsonConverter[] converters
    ) =>
        JsonConvert.SerializeObject(obj, converters);

    public static void ToJsonFile(this object obj,
                                  string      fileName,
                                  Formatting  formatting = Formatting.None
    )
    {
        var dir = Path.GetDirectoryName(fileName);

        if (string.IsNullOrWhiteSpace(dir))
        {
            throw new ApplicationException($"无效的路径 {fileName}");
        }

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var json = JsonConvert.SerializeObject(obj, formatting);
        File.WriteAllText(fileName, json);
    }

    public static void ToJsonFile(this object            obj,
                                  string                 fileName,
                                  params JsonConverter[] converters
    )
    {
        var json = JsonConvert.SerializeObject(obj, converters);
        File.WriteAllText(fileName, json);
    }

    public static string ToIndentedJson(this object obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);

    public static T CloneByJson<T>(this T source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return JsonConvert.DeserializeObject<T>(source.ToJson());
    }

    public static T FromJson<T>(string json) where T : class
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new FormatException($"文件格式错误,指定的内容不符合json格式.{Environment.NewLine}{json}");
        }

        return JsonConvert.DeserializeObject<T>(json);
    }

    public static T FromJsonFile<T>(string jsonFile) where T : class
    {
        if (string.IsNullOrWhiteSpace(jsonFile) || !File.Exists(jsonFile))
        {
            throw new FileNotFoundException("未找到指定的json文件.", jsonFile);
        }

        var json = File.ReadAllText(jsonFile);

        if (string.IsNullOrWhiteSpace(json))
        {
            throw new
                FormatException($"文件格式错误,指定的文件不符合json格式.{Environment.NewLine}文件:{Environment.NewLine}{jsonFile}{Environment.NewLine}内容:{Environment.NewLine}{json}");
        }

        return JsonConvert.DeserializeObject<T>(json);
    }

    public static void LoadFromJsonFile(this object obj, string jsonFile)
    {
        if (string.IsNullOrWhiteSpace(jsonFile) || !File.Exists(jsonFile))
        {
            throw new FileNotFoundException("未找到指定的json文件.", jsonFile);
        }

        var json = File.ReadAllText(jsonFile);

        if (string.IsNullOrWhiteSpace(json))
        {
            throw new
                FormatException($"文件格式错误,指定的文件不符合json格式.{Environment.NewLine}文件:{Environment.NewLine}{jsonFile}{Environment.NewLine}内容:{Environment.NewLine}{json}");
        }

        JsonConvert.PopulateObject(json, obj);
    }
}