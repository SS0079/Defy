using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace KittyHelpYouOut
{
	//public class JsonFileHandler
	//{
	//	private readonly string fileName;
	//	private readonly FileOptions fileOption = FileOptions.None;

	//	private JsonFileHandler(string fileName) { this.fileName = fileName; }

	//	public bool ReadFiles<T>(ref T[] dataArray)
	//	{
	//		List<T> dataList = new List<T>();
	//		if (ReadFiles(ref dataList))
	//		{
	//			dataArray = dataList.ToArray();
	//			return true;
	//		}

	//		return false;
	//	}

	//	// 读取文件到dataLine，这样就可以逐行操作
	//	public bool ReadFiles<T>(ref List<T> dataList)
	//	{
	//		try
	//		{
	//			FileStream file = File.Open(fileName, FileMode.OpenOrCreate);
	//			StreamReader reader = new StreamReader(file);
	//			dataList.Clear();
	//			while (!reader.EndOfStream)
	//			{
	//				string strLine = reader.ReadLine();
	//				dataList.Add(JsonUtility.FromJson<T>(strLine));
	//			}

	//			return true;
	//		}
	//		catch
	//		{
	//			return false;
	//		}
	//	}

	//	// 保存文件，没什么好说的
	//	public void SaveFiles(IEnumerable dataList)
	//	{
	//		FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, 8192,
	//			fileOption);
	//		StreamWriter writer = new StreamWriter(file, Encoding.Unicode);
	//		StringBuilder sb = new StringBuilder();
	//		foreach (object data in dataList)
	//		{
	//			sb.AppendLine(JsonUtility.ToJson(data));
	//		}

	//		writer.Write(sb.ToString());
	//		writer.Flush();
	//	}

	//	//public static JsonSavesHelper GetSetting(string fileName)
	//	//{
	//	//	return new JsonSavesHelper(Path.Combine(PathHelper.strSaveDataPath, fileName));
	//	//}

	//	//public static JsonSavesHelper GetSave(string fileName)
	//	//{
	//	//	return new JsonSavesHelper(Path.Combine(PathHelper.strSaveDataPath, fileName));
	//	//}
	//}

	public class JsonFileHandler
	{
		//private readonly string fileName;
		//private readonly FileOptions fileOption = FileOptions.None;

		//private JsonFileHandler(string fileName) { this.fileName = fileName; }

		// 读取文件
		public static T Read<T>(string path)
		{
			if (!File.Exists(path))
			{
				Debug.LogError("读取的文件不存在！");
				return default;
			}
			string json = File.ReadAllText(path);
			return JsonUtility.FromJson<T>(json);
		}

		public static bool TryRead<T>(string path, out T result)
		{
			if (!File.Exists(path))
			{
				Debug.LogError("读取的文件不存在！");
				result = default;
				return false;
			}
			string json = File.ReadAllText(path);
			result = JsonUtility.FromJson<T>(json);
			return true;
		}

		// 保存文件，没什么好说的
		public static void Rewrite(object obj, string path)
		{
			if (!File.Exists(path))
			{
				File.Create(path).Dispose();
			}
			string json = JsonUtility.ToJson(obj, true);
			File.WriteAllText(path, json);
			Debug.Log("保存成功");
		}

	}
}