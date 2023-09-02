using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
public class ExtentsClass2
{
    //代码扩充倍率
    static float scriptNumber = 1f;
    //场景数量：
    static int sceneNum;
    //类名库文本：
    static string path = Application.dataPath  + "/Editor/ExtentsClass/words.txt";
    //新生成类的位置：
    static string classPath = Application.dataPath + "/Scripts/SpecialScripts";
    //代码存在文件夹
    static string myClassPath= Application.dataPath+ "/Scripts";
    //类名库：
    static string[] listClass;
	//命名空间：
	static string nameSpace;
    //存取UI类型的集合
    static Dictionary<string,string> dictUIType;
    //命名空间名称
    static string nameSpaceName;
    //是否可以挂载
    static bool isClick;
    /// <summary>
    /// 垃圾代码类中的控制变量，用于控制垃圾方法的具体行为
    /// </summary>
    static string forLoopCycleCount = "forLoopCycleCount";
    static string whileLoopCycleCount = "whileLoopCycleCount";
    static string openLoop = "openForLoop";
	static string openWhile = "openWhile";
	static string openIfElse = "openIfElse";
    //原先代码数量
    static int classNumberBefore;
    //当前类
    static string currentClassName;
    //类名与方法名对应的集合
    static Dictionary<string, string> dict_ClassNameAndFunctionName;
    //存取名字的集合
    static System.Collections.Generic.List<string> list_ClassName=new System.Collections.Generic.List<string> ();
	[MenuItem("Tools/\"加入垃圾代码\"/2、多个场景挂载")]
    public static  void Start()
    {
        //初始化:
        Init();
        //0、删除原有文件夹的文件:
        ClearAllClass();
        //1、判断项目中已有多少个脚本:
        int rangeIndex = UnityEngine.Random.Range(20,50);
        int currentScriptsNumber = (int)Mathf.Round(scriptNumber * FindProjectClassNumber());
        classNumberBefore =Mathf.Clamp(currentScriptsNumber, rangeIndex, currentScriptsNumber);
        //2、加载所有类名文本
        ReadClassName();
        //3、确定命名空间：
        nameSpace = CreateNameSpace();
        //4、实例化指定个数的类文件
        RandomClassName(classNumberBefore);
        //5、创建调用类
        CreateNewClass();
    }

  
    /// <summary>
    /// 初始化
    /// </summary>
    private static void Init()
    {
        isClick = true;
        sceneNum = EditorBuildSettings.scenes.Length;
        Debug.Log("获取场景数量："+sceneNum);
        currentClassName = "";
        dict_ClassNameAndFunctionName = new Dictionary<string, string>();
        dictUIType = new Dictionary<string, string>();
        if (!Directory.Exists(myClassPath))
        {
            Directory.CreateDirectory(myClassPath);
        }
    }

    /// <summary>
    /// 1、清除原有的文件夹
    /// </summary>
    private static void ClearAllClass()
    {
        if (Directory.Exists(classPath))
        {
            Directory.Delete(classPath, true);
        }
    }

    /// <summary>
    /// 2、查找项目中的脚本个数
    /// </summary>
    private static int FindProjectClassNumber()
    {
        int num = 0;
        FindDirectoryToAllClass(myClassPath,ref num);
        return num;
    }

    /// <summary>
    /// 寻找文件夹中的类
    /// </summary>
    /// <param name=""></param>
    private static void FindDirectoryToAllClass(string path,ref int num)
    {
        try
        {
            //获取文件夹列表
            string[] dir = Directory.GetDirectories(path);
            //定位到指定的文件夹中
            DirectoryInfo fdir = new DirectoryInfo(path);
            FileInfo[] file = fdir.GetFiles();
            //当前目录文件或文件夹不为空
            if (file.Length != 0 || dir.Length != 0)
            {
                foreach (var f in file)
                {
                    if (f.Extension.Equals(".cs"))
                    {
                        Debug.Log("类名："+f.Name.Split('.')[0]);
                        num++;
                    }
                }
                foreach (var d in dir)
                {
                    FindDirectoryToAllClass(d, ref num);//递归
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    /// <summary>
    /// 3、读取所有类名文本
    /// </summary>
    private static void ReadClassName()
    {
        listClass= File.ReadAllLines(path);  
    }

    /// <summary>
    /// 4、随机类名，且创建对应类名的文件
    /// </summary>
    private static void RandomClassName(int num)
    {
        //类名
        string className = "";
        //哈希随机
        Hashtable hs = new Hashtable();
        //取num个不同类名的对象
        for (int i = 0; hs.Count < num; i++)
        {
            //类名的组拼个数：
            int index = UnityEngine.Random.Range(2, 5);
            //待组拼的类名
            string content = "";
            //****执行类名组拼***//
            for (int j = 0; j < index; j++)
            {
                //类名库的随机索引
                int rg = UnityEngine.Random.Range(0, listClass.Length);
                string ca = listClass[rg][0].ToString().ToUpper();
                string na = listClass[rg].Substring(1, listClass[rg].Length - 1);
                content = ca + na;
                className += content;
            }
            //如果hsa中不存在此类名：
            if (!hs.ContainsValue(className))
            {
                hs.Add(className, className);
                //为当前类赋值
                currentClassName = className;
                //创建对应名称的类
                CreateClassFile(className);
                className = "";
                dictUIType = new Dictionary<string, string>();
            }
        }
    }

    /// <summary>
    /// 创建对于名称的类文件
    /// </summary>
    private static void CreateClassFile(string className)
    {
        if (!Directory.Exists(classPath))
        {
            Directory.CreateDirectory(classPath);
        }

        //类文件
        string classFilePath = string.Format("{0}/{1}.cs", classPath, className);
        FileStream fs = new FileStream(classFilePath,FileMode.OpenOrCreate);
        if (!fs.CanWrite)
        {
            Debug.Log("无法写入文件");
            return;
        }

        //文本内容：
        string data = CreateClass(className);
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        //Debug.Log("字节长度："+bytes.Length);
        fs.Write(bytes,0,bytes.Length);
        fs.Flush();
        fs.Close();
    }

    /// <summary>
	/// 创建类文本内容
	/// </summary>
	/// <param name="className"></param>
	/// <returns></returns>
    private static  string CreateClass(string className)
    {
        StringBuilder sb = new StringBuilder();
        //1、应用库：
        sb.Append(CreateUsing());
        //2、命名空间：
        sb.Append(nameSpace);
        //3、生成枚举
        sb.Append(CreateEnum());
        //3、类名头文本：
        int index = UnityEngine.Random.Range(1, 10);
        if (index%2==0)
        {
            sb.Append(CreateClassHead(false, className));
        }
        else
        {
            sb.Append(CreateClassHead(false, className));
        }
        //4、字段：
        sb.Append(CreateControlVariables());
        //5、方法：
        //**方法个数 * *//
        index = Random.Range(1, 4);
		for (int i = 0; i < index; i++)
		{
			int j = Random.Range(20, 50);
			bool bo = i % 2 == 0;
			//获取返回类型
			string type = GetFunctionType();
			sb.Append(CreateMethod(bo, type));
		}
		sb.Append("\n\t}\n}");
		return sb.ToString();
    }

	/// <summary>
	/// 生成应用库
	/// </summary>
	/// <returns></returns>
	private static string CreateUsing()
	{
		StringBuilder sb = new StringBuilder();
		string[] arrStr = { "System", "System.Collections", "System.Collections.Generic", "UnityEngine.UI" };
		int index = UnityEngine.Random.Range(1, arrStr.Length + 1);
		string target = "using ";
		string content = target + "UnityEngine;"+GetHuanHang(1);
		Hashtable hs = new Hashtable();
		for (int i = 0; hs.Count < index; i++)
		{
			int num = UnityEngine.Random.Range(0, arrStr.Length);
			if (!hs.ContainsValue(num))
			{
				hs.Add(num, num);

				content += target + arrStr[num] + ";"+ GetHuanHang(1);
			}
		}
        sb.Append(content);
		return sb.ToString();
	}

    /// <summary>
	/// 创建命名空间
	/// </summary>
	/// <returns></returns>
    private static string CreateNameSpace()
	{
        int index = Random.Range(0, listClass.Length);
        string ca = listClass[index][0].ToString().ToUpper();
        string na = listClass[index].Substring(1, listClass[index].Length - 1);
        nameSpaceName = ca + na;
        string content = "namespace " + nameSpaceName + GetHuanHang(1) + "{";
        return content;
	}

	/// <summary>
	/// 获取返回类型
	/// </summary>
	/// <returns></returns>
	private static string GetFunctionType()
	{
		int index = UnityEngine.Random.Range(1, 6);
		switch (index)
		{
			case 1:
				return "int";
			case 2:
				return "string";
			case 3:
				return "long";
			case 4:
				return "object";
			case 5:
				return "Vector3";
			default:
				return "Vector2";
		}
	}

	/// <summary>
	/// 创建类的头文本
	/// </summary>
	/// <param name="implaementMono"></param>
	/// <param name="calssName"></param>
	/// <returns></returns>
	private static string CreateClassHead(bool implaementMono, string calssName)
	{
		string str = implaementMono ? ": MonoBehaviour " : "";
        string sss = implaementMono ? " " : " static ";
        return "\n\tpublic "+sss+" class "+calssName + str + GetHuanHang(1) +GetSuoJin(1)+ "{";
	}


    /// <summary>
	/// 随机方法名称
	/// </summary>
	private static string RandomFunctionName()
	{
		string functionName="";
        string[] one = { "Get", "User", "Set", "Random" };
		string[] two = { "By", "From", "To", "In", "On", "At", "As", "For", "And", "With" };
		string[] there = { "Name", "ID", "Type","Index","Datas" };
        functionName += one[RandomIndex(one.Length)] + listClass[RandomIndex(listClass.Length)];
        functionName += two[RandomIndex(two.Length)]+there[RandomIndex(there.Length)];
        return functionName;
	}


    /// <summary>
    /// 创建随机函数
    /// </summary>
    /// <param name="hasReturnValue"></param>
    /// <param name="returnValueType"></param>
    /// <param name="methodNamePrefix"></param>
    /// <param name="totalLine"></param>
    /// <returns></returns>
    private static string CreateMethod(bool isType,string type)
    { 
        StringBuilder sb = new StringBuilder();
        //随机方法名
        string funName = RandomFunctionName();
        if (!dict_ClassNameAndFunctionName.ContainsKey(currentClassName))
        {
            dict_ClassNameAndFunctionName.Add(currentClassName,funName);
        }
        //函数头文本：
        sb.Append(CreateFunctionHead(isType,type,funName));
        //函数内容：
        sb.Append(CreateFunctionContent(isType,type));
        sb.Append("\n\t\t}");
        return sb.ToString();
    }


    /// <summary>
    /// 创建函数头文本
    /// </summary>
    /// <param name="isType">是否有返回值</param>
    /// <param name="type">返回值</param>
    /// <param name="funName">函数名</param>
    /// <returns></returns>
    private static string CreateFunctionHead(bool isType, string type, string funName)
    {
        StringBuilder sb = new StringBuilder();
        string returnStr = isType ?  type: "void";
        string content = "\n\t\tpublic static " + returnStr + " " + funName + "() \n\t\t{";
        sb.Append(content);
        return sb.ToString();
    }
	
	/// <summary>
	/// 创建函数体内容
	/// </summary>
	/// <returns></returns>
	private static string CreateFunctionContent(bool isType,string type)
	{
        //得到返回语句
        string returnStratement;
        if (isType)
        {
            returnStratement = CreateReturnStatement(type);
        }
        else
        {
            returnStratement = "";
        }

        //代码行数
        int lineIndex=Random.Range(10, 21);
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < lineIndex; i++)
		{
            int typeIndex = Random.Range(0,4);
            
            switch (typeIndex)
            {
                case 0://for循环
                    //随机语句内的内容
                    string fhunctionConcent = CreateSingleRandomStatement();
                    string content= CreateForLoop(fhunctionConcent);
                    sb.Append(content);
                    break;
                case 1:
                    string fhunctionConcent_1 = CreateSingleRandomStatement();
                    string fhunctionConcent_2 = CreateSingleRandomStatement();
                    content = CreateIfElse(fhunctionConcent_1, fhunctionConcent_2);
                    sb.Append(content);
                    break;
                case 2:
                    fhunctionConcent = CreateSingleRandomStatement();
                    content = CreateWhile(fhunctionConcent);
                    sb.Append(content);
                    break;
                case 3:
                    //ui组件获取：
                    content = CreateUIFunction();
                    sb.Append(content);
                    break;
                default:
                    break;
            }   
        }
        sb.Append(returnStratement);
        return sb.ToString();
	}

    /// <summary>
    /// 随意字段，语句内
    /// </summary>
    /// <returns></returns>
    static string CreateSingleRandomStatement()
    {
        int indes = Random.Range(0,4);
        byte[] arrBty = new byte[1];
        Hashtable has = new Hashtable();
        int num = 0;
        string Name=  RandomFunction(ref has, ref num, 2);
        int index = Random.Range(0, listClass.Length);
        switch (indes)
        {
            case 0:
                return string.Format("int {0} = UnityEngine.Random.Range(0,{1});\n\t\t\t\t{2}++;", Name,index, Name);
            case 1:
                return string.Format("string {0}=\"{1}\";\n\t\t\t\t{2}=\"\";", Name, listClass[index],Name); 
            case 2:
                return string.Format("var {0}=\"{1}\";\n\t\t\t\t{2}=\"\";", Name, listClass[index],Name);
            case 3:
                return string.Format("int {0}={1};\n\t\t\t\t{2}++;", Name,index,Name);
            case 4:
                return string.Format("double {0}={1};\n\t\t\t\t{2}--;", Name,index,Name);
            case 5:
                return string.Format("byte[] {0}= new byte[{1}];\n\t\t\t\t{2}=null;", Name,index,Name);
            default:
                return string.Format("float {0}={1}f;\n\t\t\t\t{2}--;", Name,index,Name);
        }
    }

    #region 字段格式
    /// <summary>
    /// 创建类的控制类变量，包含：
    /// 1.for循环次数
    /// 2.是否开启循环
    /// 3.是否开启switch语句
    /// 4.是否开启判断语句
    /// </summary>
    /// <returns></returns>
    static string CreateControlVariables()
    {
        Hashtable has = new Hashtable();
        int index = 0;
        //1、For循环：
        forLoopCycleCount = RandomFunction(ref has, ref index, 2);
        int num = UnityEngine.Random.Range(1,10000000);
        int publicOrPrivate = Random.Range(1,10);
        string temp_Str = publicOrPrivate % 2 == 0 ? "public " : "private "; 
        string _forLoop = "\n\t\t"+ temp_Str + "static int " + forLoopCycleCount + " ="+num +";";

        //2、是否打开循环：
        openLoop = RandomFunction(ref has, ref index, 2);
        publicOrPrivate = Random.Range(1, 10);
        temp_Str = publicOrPrivate % 2 == 0 ? "public " : "private ";
        int isok= UnityEngine.Random.Range(0, 10);
        string sss= isok >= 6 ? "true" : "false";
        string _openLoop = "\n\t\t"+ temp_Str + "static bool " + openLoop + " =false"  + ";";

        //3、是否打开while循环：
        openWhile = RandomFunction(ref has, ref index, 2);
        isok = UnityEngine.Random.Range(0, 10);
        publicOrPrivate = Random.Range(1, 10);
        temp_Str = publicOrPrivate % 2 == 0 ? "public " : "private ";
        string _openWhile = "\n\t\t"+ temp_Str + "static bool " + openWhile+ " =false"+ ";";

        //4、是否打开 if-else 判断：
        openIfElse = RandomFunction(ref has, ref index, 2);
        isok = UnityEngine.Random.Range(0, 10);
        publicOrPrivate = Random.Range(1, 10);
        temp_Str = publicOrPrivate % 2 == 0 ? "public " : "private ";
        sss = isok >= 6 ? "true" : "false";
        string _openIfElse = "\n\t\t"+ temp_Str + "static bool " + openIfElse + " =" + sss + ";";

        //5、while循环次数限定：
        whileLoopCycleCount = RandomFunction(ref has, ref index, 2);
        num = UnityEngine.Random.Range(1, 10000000);
        publicOrPrivate = Random.Range(1, 10);
        temp_Str = publicOrPrivate % 2 == 0 ? "public " : "private ";
        string _whileLoop = "\n\t\t"+ temp_Str + "static int " +whileLoopCycleCount + " ="+ num + ";";
        int rag = Random.Range(1, 5);

        //6、委托语句
        string _delegate = "";
        for (int i = 0; i < rag; i++)
        {
            publicOrPrivate = Random.Range(1, 10);
            temp_Str = publicOrPrivate % 2 == 0 ? "public " : "private ";
            _delegate += string.Format("\n\t\t"+ temp_Str + "delegate void {0}();", RandomFunction(ref has, ref index, 2));
        }

        //7、常量语句：
        int rge = Random.Range(1,5);
        string _count = "";
        for (int i = 0; i < rge; i++)
        {
            publicOrPrivate = Random.Range(1, 10);
            temp_Str = publicOrPrivate % 2 == 0 ? "public " : "private ";
            _count += string.Format("\n\t\t"+ temp_Str + "const string {0}= \"{1}\";", RandomFunction(ref has, ref index, 2), RandomFunction(ref has, ref index, 2));
        }
        //8、ui组件变量：
        string[] arrUI = {"Button","Text","Image","RawImage","Toggle","Slider","Scrollbar","Dropdown","InputField", "ScrollRect" };
        int NUMBER = 0;
        //ui组件内容：
        string _UIContent = "";
        for (int i = 0; i < arrUI.Length; i++)
        {
            NUMBER = Random.Range(0, arrUI.Length);
            publicOrPrivate = Random.Range(1, 10);
            temp_Str = publicOrPrivate % 2 == 0 ? "public " : "private ";
            string uiName= RandomFunction(ref has, ref index, 2);
            string type = arrUI[NUMBER];
            if (!dictUIType.ContainsKey(type))
            {
                dictUIType.Add(type, uiName);
            }
            _UIContent += "\n\t\t" + temp_Str + "static UnityEngine.UI." + type + " "+ uiName+";";
        }
        return _forLoop + _openLoop + _openWhile + _openIfElse + _whileLoop+ _delegate+ _count + _UIContent + "\n"; 
    }
    #endregion


    private static string RandomFunction(ref Hashtable hs,ref int index,int nnn)
    {
        string content = "";
        index += nnn;
        for (int i = 0; hs.Count < index; i++)
        {
            int num = Random.Range(0,listClass.Length);
            if (!hs.ContainsKey(listClass[num]))
            {     
                hs.Add(listClass[num],num);
                string ca = listClass[num][0].ToString().ToUpper();
                string na = listClass[num].Substring(1, listClass[num].Length - 1);
                content += ca + na;
            }
        }
        return content;
    }

    #region 创建for循环、if语句、while语句、返回语句 枚举 的格式 

    /// <summary>
    /// 生成枚举类型
    /// </summary>
    /// <param name="nums"></param>
    /// <returns></returns>
    public static string CreateEnum()
    {
        int nums = Random.Range(2,10);
        if (nums % 2 == 0) return"";
        Hashtable has = new Hashtable();
        int num = 0;
        string Name = RandomFunction(ref has, ref num, 2);
        string title = "\npublic enum " + Name + " \n{\n";
        string content = "";
        string target = ",\n";
        for (int i = 0; i < nums; i++)
        {
            if ((nums - 1).Equals(i)) target = "";
            int index = Random.Range(2,4);
            Name = RandomFunction(ref has, ref num, index);
            content +="\t"+Name + target;
        }
        content += "\n}";
        return title + content;
    }

    /// <summary>
    /// if--else语句
    /// </summary>
    /// <param name="ifString">if语句内容</param>
    /// <param name="elseString">else语句内容</param>
    /// <returns></returns>
    private static string CreateIfElse(string ifString, string elseString)
    {
        string IfString = "\n\n\n\t\t\tif(" + openIfElse + ")\n\t\t\t{\n\t\t\t\t" + ifString + "\n\t\t\t}\n\t\t\telse\n\t\t\t{\n\t\t\t\t" + elseString + "\n\t\t\t}";
        return IfString;
    }

    /// <summary>
	/// for循环语句
	/// </summary>
	/// <param name="forContent"></param>
	private static string CreateForLoop(string forContent)
    {
        string forString = "\n\n\t\t\tif(" + openLoop + ")\n\t\t\t{\n\t\t\t\tfor(int i = 0;i<" + forLoopCycleCount + ";i++)\n\t\t\t\t{\n\t\t\t\t\t" + forContent + "\n\t\t\t\t}\n\t\t\t}";
        return forString;
    }

    /// <summary>
    /// 创建while循环
    /// </summary>
    /// <param name="whileStr">while循环中要执行的东西</param>
    /// <returns></returns>
    private static string CreateWhile(string whileStr)
    {
        return "\n\n\t\t\tif(" + openWhile + ")\n\t\t\t{\n\t\t\t\tint i =0;\n\t\t\t\twhile(i<" + whileLoopCycleCount + ")\n\t\t\t\t{\n\t\t\t\t\t" + whileStr + "\n\t\t\t\t}\n\t\t\t}";
    }

    /// <summary>
    /// 创建返回语句
    /// </summary>
    /// <param name="returnValueType"></param>
    /// <param name="suojin"></param>
    /// <returns></returns>
    private static string CreateReturnStatement(string returnValueType)
    {
        return GetHuanHang(1) + GetSuoJin(2) + "return default(" + returnValueType + ");";
    }

    /// <summary>
    /// 创建UI组件操作函数
    /// </summary>
    /// <returns></returns>
    private static string CreateUIFunction()
    {
        string content =GetHuanHang(1) + GetSuoJin(2);
        // string[] arrUI = { , "Text",, "", "", "", "", "", "", "ScrollRect" };
        foreach (var uiType in dictUIType)
        {
            switch (uiType.Key)
            {
                case "Button":
                    string temp1 = CreateSingleRandomStatement();
                    string temp2 = CreateSingleRandomStatement();
                    string str = GetSuoJin(2)+uiType.Value + ".onClick.AddListener(()=>{"+ GetHuanHang(1)+GetSuoJin(4) + temp1 + GetSuoJin(4) + GetHuanHang(1) + GetSuoJin(4)+"});";
                    //Debug.Log("Button:"+str);
                    content += GetSuoJin(2)+"if("+ uiType.Value+"==null){"+GetHuanHang(1)+GetSuoJin(4)+temp2+GetHuanHang(1)+GetSuoJin(4)+"}"+GetHuanHang(1)+GetSuoJin(4)+"else{"+GetHuanHang(1)+GetSuoJin(2)+str+GetHuanHang(1)+GetSuoJin(4)+"}" + GetHuanHang(1);
                    break;
                case "Text":
                    temp1 = CreateSingleRandomStatement();
                    temp2 = CreateSingleRandomStatement();
                    int index = Random.Range(1,100);
                    str = GetSuoJin(2) + uiType.Value + ".text=" + index+ ".ToString();";
                   // Debug.Log("Button:" + str);
                    content += GetSuoJin(2) + "if(" + uiType.Value + "==null){" + GetHuanHang(1) + GetSuoJin(4) + temp2 + GetHuanHang(1) + GetSuoJin(4) + "}" + GetHuanHang(1) + GetSuoJin(4) + "else{" + GetHuanHang(1) + GetSuoJin(2) + str + GetHuanHang(1) + GetSuoJin(4) + "}" + GetHuanHang(1);
                    break;
                case "Image":
                    temp1 = CreateSingleRandomStatement();
                    temp2 = CreateSingleRandomStatement();
                    str = GetSuoJin(2) + uiType.Value + ".sprite=null;";
                    //Debug.Log("Button:" + str);
                    content += GetSuoJin(2) + "if(" + uiType.Value + "==null){" + GetHuanHang(1) + GetSuoJin(4) + temp2 + GetHuanHang(1) + GetSuoJin(4) + "}" + GetHuanHang(1) + GetSuoJin(4) + "else{" + GetHuanHang(1) + GetSuoJin(2) + str + GetHuanHang(1) + GetSuoJin(4) + "}" + GetHuanHang(1);
                    break;
                case "Slider":
                    temp1 = CreateSingleRandomStatement();
                    temp2 = CreateSingleRandomStatement();
                    str = GetSuoJin(2) + uiType.Value +"=null;";
                    content += GetSuoJin(2) + "if(" + uiType.Value + "==null){" + GetHuanHang(1) + GetSuoJin(4) + temp2 + GetHuanHang(1) + GetSuoJin(4) + "}" + GetHuanHang(1) + GetSuoJin(4) + "else{" + GetHuanHang(1) + GetSuoJin(2) + str + GetHuanHang(1) + GetSuoJin(4) + "}" + GetHuanHang(1);
                    break;
                case "RawImage":
                    temp1 = CreateSingleRandomStatement();
                    temp2 = CreateSingleRandomStatement();
                    str = GetSuoJin(2) + uiType.Value + "=null;";
                    content += GetSuoJin(2) + "if(" + uiType.Value + "==null){" + GetHuanHang(1) + GetSuoJin(4) + temp2 + GetHuanHang(1) + GetSuoJin(4) + "}" + GetHuanHang(1) + GetSuoJin(4) + "else{" + GetHuanHang(1) + GetSuoJin(2) + str + GetHuanHang(1) + GetSuoJin(4) + "}" + GetHuanHang(1);
                    break;
                case "Toggle":
                    temp1 = CreateSingleRandomStatement();
                    temp2 = CreateSingleRandomStatement();
                    str = GetSuoJin(2) + uiType.Value + "=null;";
                    content += GetSuoJin(2) + "if(" + uiType.Value + "==null){" + GetHuanHang(1) + GetSuoJin(4) + temp2 + GetHuanHang(1) + GetSuoJin(4) + "}" + GetHuanHang(1) + GetSuoJin(4) + "else{" + GetHuanHang(1) + GetSuoJin(2) + str + GetHuanHang(1) + GetSuoJin(4) + "}" +GetHuanHang(1);
                    break;
                case "Dropdown":
                    temp1 = CreateSingleRandomStatement();
                    temp2 = CreateSingleRandomStatement();
                    str = GetSuoJin(2) + uiType.Value + "=null;";
                    content += GetSuoJin(2) + "if(" + uiType.Value + "==null){" + GetHuanHang(1) + GetSuoJin(4) + temp2 + GetHuanHang(1) + GetSuoJin(4) + "}" + GetHuanHang(1) + GetSuoJin(4) + "else{" + GetHuanHang(1) + GetSuoJin(2) + str + GetHuanHang(1) + GetSuoJin(4) + "}" + GetHuanHang(1);
                    break;
                case "InputField":
                    temp1 = CreateSingleRandomStatement();
                    temp2 = CreateSingleRandomStatement();
                    str = GetSuoJin(2) + uiType.Value + "=null;";
                    content += GetSuoJin(2) + "if(" + uiType.Value + "==null){" + GetHuanHang(1) + GetSuoJin(4) + temp2 + GetHuanHang(1) + GetSuoJin(4) + "}" + GetHuanHang(1) + GetSuoJin(4) + "else{" + GetHuanHang(1) + GetSuoJin(2) + str + GetHuanHang(1) + GetSuoJin(4) + "}"+GetHuanHang(1);
                    break;
                case "ScrollRect":
                    temp1 = CreateSingleRandomStatement();
                    temp2 = CreateSingleRandomStatement();
                    str = GetSuoJin(2) + uiType.Value + "=null;";
                    content += GetSuoJin(2) + "if(" + uiType.Value + "==null){" + GetHuanHang(1) + GetSuoJin(4) + temp2 + GetHuanHang(1) + GetSuoJin(4) + "}" + GetHuanHang(1) + GetSuoJin(4) + "else{" + GetHuanHang(1) + GetSuoJin(2) + str + GetHuanHang(1) + GetSuoJin(4) + "}"+GetHuanHang(1);
                    break;

            }
        }
        return content+GetHuanHang(1);
    }

    #endregion

    #region 代码工具

    /// <summary>
    /// 随机数
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    private static int RandomIndex(int length)
	{
		return Random.Range(0, length);
	}

	/// <summary>
	/// 换行
	/// </summary>
	/// <param name="num"></param>
	/// <returns></returns>
	private static string GetHuanHang(int num)
	{
		if (num <= 0)
		{
			return "";
		}
		string huanhang = string.Empty;
		for (int i = 0; i < num; i++)
		{
			huanhang += "\n";
		}
		return huanhang;
	}

	/// <summary>
	/// 获取缩进的字符串
	/// </summary>
	/// <param name="suojin"></param>
	/// <returns></returns>
	private static string GetSuoJin(int suojin)
	{
		if (suojin <= 0)
		{
			return "";
		}
		string suojinstr = string.Empty;

		for (int i = 0; i < suojin; i++)
		{
			suojinstr += "\t";
		}
		return suojinstr;
	}
    #endregion

    #region 实例化一个类来调字典里的类方法
    private static void CreateNewClass()
    {
        //动态生成的脚本名字
        string new_ScriptsName = nameSpaceName+"&";
        string path = classPath + "/AutoClass";
        //创建文件夹
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        //把代码几等分
        int scriptsNum = dict_ClassNameAndFunctionName.Count / sceneNum;
        //阶段目标索引
        int baseIndex = 0;
        //当前次数
        int curIndex = 0;
        for (int i = 0; i < sceneNum; i++)
        {
            //1、随机一个类名：
            string className = "";
            //****执行类名组拼***//
            for (int j = 0; j < 4; j++)
            {
                //类名库的随机索引
                int rg = UnityEngine.Random.Range(0, listClass.Length);
                string ca = listClass[rg][0].ToString().ToUpper();
                string na = listClass[rg].Substring(1, listClass[rg].Length - 1);
                string content = ca + na;
                className += content;
            }
            //2、引入命名空间
            StringBuilder sb = new StringBuilder();
            //1、应用库：
            sb.Append(CreateUsing());
            //2、命名空间：
            sb.Append(nameSpace);
            //3、创建类名头文本
            string headContent = CreateClassHead(true, className.Trim());
            sb.Append(headContent);
            //4、创建start方法
            string funName = "";
            //4.1、随机一个方法名：
            for (int j = 0; j < 3; j++)
            {
                //类名库的随机索引
                int rg = UnityEngine.Random.Range(0, listClass.Length);
                string ca = listClass[rg][0].ToString().ToUpper();
                string na = listClass[rg].Substring(1, listClass[rg].Length - 1);
                string content = ca + na;
                funName += content;
            }
            string fun = CreateFunctionHead(true, "System.Collections.IEnumerator", funName);
            fun = fun.Replace("static", "");//去除静态函数
            sb.Append(fun);


            //5、写入类方法调用：
            string str = "\n";//Start方法内部的内容
            str += "\t\t\tfloat index=1f;\n";

            baseIndex = scriptsNum*(i+1);
            //存取已用的方法名
            List<string> listScripted = new List<string>();
            foreach (var item in dict_ClassNameAndFunctionName)
            {
                if (!listScripted.Contains(item.Key))
                {
                    listScripted.Add(item.Key);
                }
                float num = Random.Range(0f,5f);
                str += string.Format("\t\t\t index = UnityEngine.Random.Range(0,{0}f);\n",num); 
                str += "\t\t\t" + item.Key + "." + item.Value + "();\n";
                str += "\t\t\tyield return new WaitForSeconds(index);\n";
                curIndex++;
                if (curIndex >= baseIndex) break;
            }

            //移除已有的方法名
            for (int k = 0; k < listScripted.Count; k++)
            {
                if (dict_ClassNameAndFunctionName.ContainsKey(listScripted[i]))
                {
                    dict_ClassNameAndFunctionName.Remove(listScripted[i]);
                }
            }

            sb.Append(str + "\n\t\t}");

            //创建Start方法
            string startFun = CreateFunctionHead(false, "", "Start");
            startFun = startFun.Replace("static", "");
            sb.Append(startFun);
            string con = "\n\t\t\tStartCoroutine(" + funName + "());";
            sb.Append(con + "\n\t\t}");
            //6、结尾
            sb.Append("\n\t}\n}");
            //写入类文件
            string classFilePath = string.Format("{0}/{1}.cs", path, className);
            FileStream fs = new FileStream(classFilePath, FileMode.OpenOrCreate);
            if (!fs.CanWrite)
            {
                Debug.Log("无法写入文件");
                return;
            }
            //文本内容：
            string data = sb.ToString();
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            //Debug.Log("字节长度："+bytes.Length);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
            fs.Close();
            new_ScriptsName += className + "|";
        }
        new_ScriptsName = new_ScriptsName.Substring(0, new_ScriptsName.Length-1);
        #region 针对已编译的函数的做法
        //foreach(var scene in EditorBuildSettings.scenes)
        //{
        //    Scene scene1 = EditorSceneManager.OpenScene(scene.path);
        //    string newName = "Camera2DProject.CameraAdaptation" + "." + "CameraAdaptation";
        //    GameObject go = new GameObject("CameraAdaptation");
        //    var asmbly = Assembly.Load("Assembly-CSharp");
        //    go.AddComponent(asmbly.GetType(newName));
        //    EditorSceneManager.SaveScene(scene1);
        //}
        #endregion
        foreach (var item in EditorBuildSettings.scenes)
        {
            EditorSceneManager.OpenScene(item.path, OpenSceneMode.Additive);
        }

        CallBacks.Set<ExtentsClass2>("Move", new_ScriptsName);
    }

    /// <summary>
    /// 动态挂载脚本
    /// </summary>
    /// <param name="scriptsName"></param>
    public static void Move(string scriptsName)
    {
        Debug.Log("scriptsName:" + scriptsName);
        string nameSpaceName = scriptsName.Split('&')[0];
        string[] arrScripts = scriptsName.Split('&')[1].Split('|');
        for (int i = 0; i < arrScripts.Length; i++)
        {
            Scene scene= EditorSceneManager.GetSceneByPath(EditorBuildSettings.scenes[i].path);
            string newName = nameSpaceName + "." + arrScripts[i];
            GameObject go = new GameObject(arrScripts[i]);
            var asmbly = Assembly.Load("Assembly-CSharp");
            go.AddComponent(asmbly.GetType(newName));
            EditorSceneManager.MoveGameObjectToScene(go, scene);
            EditorSceneManager.SaveScene(scene);             
        }

        //关闭打开的场景
        for (int j = 0; j < EditorBuildSettings.scenes.Length; j++)
        {
            Scene scene = EditorSceneManager.GetSceneByPath(EditorBuildSettings.scenes[j].path);
            EditorSceneManager.CloseScene(scene,true);


        }
       
    }

    #endregion
}
