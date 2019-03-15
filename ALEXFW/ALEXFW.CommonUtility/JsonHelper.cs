using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ALEXFW.CommonUtility
{
    /// <summary>
    ///     用于构建属性值的回调
    /// </summary>
    /// <param name="Property"></param>
    public delegate void SetProperties(JsonObject Property);

    /// <summary>
    ///     JsonObject属性值类型
    /// </summary>
    public enum JsonPropertyType
    {
        String,
        Object,
        Array,
        Number,
        Bool,
        Null
    }

    /// <summary>
    ///     JSON通用对象
    /// </summary>
    public class JsonObject
    {
        private Dictionary<string, JsonProperty> _property;

        public JsonObject()
        {
            _property = null;
        }

        public JsonObject(string jsonString)
        {
            Parse(ref jsonString);
        }

        public JsonObject(SetProperties callback)
        {
            if (callback != null)
            {
                callback(this);
            }
        }

        /// <summary>
        ///     获取属性
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public JsonProperty this[string PropertyName]
        {
            get
            {
                JsonProperty result = null;
                if (_property != null && _property.ContainsKey(PropertyName))
                {
                    result = _property[PropertyName];
                }
                return result;
            }
            set
            {
                if (_property == null)
                {
                    _property = new Dictionary<string, JsonProperty>(StringComparer.OrdinalIgnoreCase);
                }
                if (_property.ContainsKey(PropertyName))
                {
                    _property[PropertyName] = value;
                }
                else
                {
                    _property.Add(PropertyName, value);
                }
            }
        }

        /// <summary>
        ///     Json字符串解析
        /// </summary>
        /// <param name="jsonString"></param>
        private void Parse(ref string jsonString)
        {
            var len = jsonString.Length;
            var poo = string.IsNullOrEmpty(jsonString);
            var po = jsonString.Substring(0, 1);
            var ll = jsonString.Substring(jsonString.Length - 1, 1);
            if (string.IsNullOrEmpty(jsonString) || jsonString.Substring(0, 1) != "{" ||
                jsonString.Substring(jsonString.Length - 1, 1) != "}")
            {
                throw new ArgumentException("传入文本不符合Json格式!" + jsonString);
            }
            var stack = new Stack<char>();
            var stackType = new Stack<char>();
            var sb = new StringBuilder();
            char cur;
            var convert = false;
            var isValue = false;
            JsonProperty last = null;
            for (var i = 1; i <= len - 2; i++)
            {
                cur = jsonString[i];
                if (cur == '}')
                {
                    ;
                }
                if (cur == ' ' && stack.Count == 0)
                {
                    ;
                }
                else if ((cur == '\'' || cur == '\"') && !convert && stack.Count == 0 && !isValue)
                {
                    sb.Length = 0;
                    stack.Push(cur);
                }
                else if ((cur == '\'' || cur == '\"') && !convert && stack.Count > 0 && stack.Peek() == cur &&
                         !isValue)
                {
                    stack.Pop();
                }
                else if ((cur == '[' || cur == '{') && stack.Count == 0)
                {
                    stackType.Push(cur == '[' ? ']' : '}');
                    sb.Append(cur);
                }
                else if ((cur == ']' || cur == '}') && stack.Count == 0 && stackType.Peek() == cur)
                {
                    stackType.Pop();
                    sb.Append(cur);
                }
                else if (cur == ':' && stack.Count == 0 && stackType.Count == 0 && !isValue)
                {
                    last = new JsonProperty();
                    this[sb.ToString()] = last;
                    isValue = true;
                    sb.Length = 0;
                }
                else if (cur == ',' && stack.Count == 0 && stackType.Count == 0)
                {
                    if (last != null)
                    {
                        var temp = sb.ToString();
                        last.Parse(ref temp);
                    }
                    isValue = false;
                    sb.Length = 0;
                }
                else
                {
                    sb.Append(cur);
                }
            }
            if (sb.Length > 0 && last != null && last.Type == JsonPropertyType.Null)
            {
                var temp = sb.ToString();
                last.Parse(ref temp);
            }
        }

        /// <summary>
        ///     通过此泛型函数可直接获取指定类型属性的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public virtual T Properties<T>(string PropertyName) where T : class
        {
            var p = this[PropertyName];
            if (p != null)
            {
                return p.GetValue<T>();
            }
            return default(T);
        }

        /// <summary>
        ///     获取属性名称列表
        /// </summary>
        /// <returns></returns>
        public string[] GetPropertyNames()
        {
            if (_property == null)
                return null;
            string[] keys = null;
            if (_property.Count > 0)
            {
                keys = new string[_property.Count];
                _property.Keys.CopyTo(keys, 0);
            }
            return keys;
        }

        /// <summary>
        ///     移除一个属性
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public JsonProperty RemoveProperty(string PropertyName)
        {
            if (_property != null && _property.ContainsKey(PropertyName))
            {
                var p = _property[PropertyName];
                _property.Remove(PropertyName);
                return p;
            }
            return null;
        }

        /// <summary>
        ///     是否为空对象
        /// </summary>
        /// <returns></returns>
        public bool IsNull()
        {
            return _property == null;
        }

        public override string ToString()
        {
            return ToString("");
        }

        /// <summary>
        ///     ToString...
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <returns></returns>
        public virtual string ToString(string format)
        {
            if (IsNull())
            {
                return "{}";
            }
            var sb = new StringBuilder();
            foreach (var key in _property.Keys)
            {
                sb.Append(",");
                sb.Append(key).Append(": ");
                sb.Append(_property[key].ToString(format));
            }
            if (_property.Count > 0)
            {
                sb.Remove(0, 1);
            }
            sb.Insert(0, "{");
            sb.Append("}");
            return sb.ToString();
        }
    }

    /// <summary>
    ///     JSON对象属性
    /// </summary>
    public class JsonProperty
    {
        private bool _bool;
        private List<JsonProperty> _list;
        private double _number;
        private JsonObject _object;
        private string _value;

        public JsonProperty()
        {
            Type = JsonPropertyType.Null;
            _value = null;
            _object = null;
            _list = null;
        }

        public JsonProperty(object value)
        {
            SetValue(value);
        }

        public JsonProperty(string jsonString)
        {
            Parse(ref jsonString);
        }

        /// <summary>
        ///     定义一个索引器，如果属性是非数组的，返回本身
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public JsonProperty this[int index]
        {
            get
            {
                JsonProperty r = null;
                if (Type == JsonPropertyType.Array)
                {
                    if (_list != null && _list.Count - 1 >= index)
                    {
                        r = _list[index];
                    }
                }
                else if (index == 0)
                {
                    return this;
                }
                return r;
            }
        }

        /// <summary>
        ///     提供一个字符串索引，简化对Object属性的访问
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public JsonProperty this[string PropertyName]
        {
            get
            {
                if (Type == JsonPropertyType.Object)
                {
                    return _object[PropertyName];
                }
                return null;
            }
            set
            {
                if (Type == JsonPropertyType.Object)
                {
                    _object[PropertyName] = value;
                }
                else
                {
                    throw new NotSupportedException("Json属性不是对象类型!");
                }
            }
        }

        /// <summary>
        ///     JsonObject值
        /// </summary>
        public JsonObject Object
        {
            get
            {
                if (Type == JsonPropertyType.Object)
                    return _object;
                return null;
            }
        }

        /// <summary>
        ///     字符串值
        /// </summary>
        public string Value
        {
            get
            {
                if (Type == JsonPropertyType.String)
                {
                    return _value;
                }
                if (Type == JsonPropertyType.Number)
                {
                    return _number.ToString();
                }
                return null;
            }
        }

        /// <summary>
        ///     Array值，如果属性是非数组的，则封装成只有一个元素的数组
        /// </summary>
        public List<JsonProperty> Items
        {
            get
            {
                if (Type == JsonPropertyType.Array)
                {
                    return _list;
                }
                var list = new List<JsonProperty>();
                list.Add(this);
                return list;
            }
        }

        /// <summary>
        ///     数值
        /// </summary>
        public double Number
        {
            get
            {
                if (Type == JsonPropertyType.Number)
                {
                    return _number;
                }
                return double.NaN;
            }
        }

        public virtual int Count
        {
            get
            {
                var c = 0;
                if (Type == JsonPropertyType.Array)
                {
                    if (_list != null)
                    {
                        c = _list.Count;
                    }
                }
                else
                {
                    c = 1;
                }
                return c;
            }
        }

        public JsonPropertyType Type { get; private set; }

        /// <summary>
        ///     Json字符串解析
        /// </summary>
        /// <param name="jsonString"></param>
        public void Parse(ref string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                SetValue(null);
            }
            else
            {
                var first = jsonString.Substring(0, 1);
                var last = jsonString.Substring(jsonString.Length - 1, 1);
                if (first == "[" && last == "]")
                {
                    SetValue(ParseArray(ref jsonString));
                }
                else if (first == "{" && last == "}")
                {
                    SetValue(ParseObject(ref jsonString));
                }
                else if ((first == "'" || first == "\"") && first == last)
                {
                    SetValue(ParseString(ref jsonString));
                }
                else if (jsonString == "true" || jsonString == "false")
                {
                    SetValue(jsonString == "true" ? true : false);
                }
                else if (jsonString == "null")
                {
                    SetValue(null);
                }
                else
                {
                    double d = 0;
                    if (double.TryParse(jsonString, out d))
                    {
                        SetValue(d);
                    }
                    else
                    {
                        SetValue(jsonString);
                    }
                }
            }
        }

        /// <summary>
        ///     Json Array解析
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private List<JsonProperty> ParseArray(ref string jsonString)
        {
            var list = new List<JsonProperty>();
            var len = jsonString.Length;
            var sb = new StringBuilder();
            var stack = new Stack<char>();
            var stackType = new Stack<char>();
            var conver = false;
            char cur;
            for (var i = 1; i <= len - 2; i++)
            {
                cur = jsonString[i];
                if (char.IsWhiteSpace(cur) && stack.Count == 0)
                {
                    ;
                }
                else if ((cur == '\'' && stack.Count == 0 && !conver && stackType.Count == 0) ||
                         (cur == '\"' && stack.Count == 0 && !conver && stackType.Count == 0))
                {
                    sb.Length = 0;
                    sb.Append(cur);
                    stack.Push(cur);
                }
                else if (cur == '\\' && stack.Count > 0 && !conver)
                {
                    sb.Append(cur);
                    conver = true;
                }
                else if (conver)
                {
                    conver = false;

                    if (cur == 'u')
                    {
                        sb.Append(new[] { cur, jsonString[i + 1], jsonString[i + 2], jsonString[i + 3] });
                        i += 4;
                    }
                    else
                    {
                        sb.Append(cur);
                    }
                }
                else if ((cur == '\'' || cur == '\"') && !conver && stack.Count > 0 && stack.Peek() == cur &&
                         stackType.Count == 0)
                {
                    sb.Append(cur);
                    list.Add(new JsonProperty(sb.ToString()));
                    stack.Pop();
                }
                else if ((cur == '[' || cur == '{') && stack.Count == 0)
                {
                    if (stackType.Count == 0)
                    {
                        sb.Length = 0;
                    }
                    sb.Append(cur);
                    stackType.Push(cur == '[' ? ']' : '}');
                }
                else if ((cur == ']' || cur == '}') && stack.Count == 0 && stackType.Count > 0 &&
                         stackType.Peek() == cur)
                {
                    sb.Append(cur);
                    stackType.Pop();
                    if (stackType.Count == 0)
                    {
                        list.Add(new JsonProperty(sb.ToString()));
                        sb.Length = 0;
                    }
                }
                else if (cur == ',' && stack.Count == 0 && stackType.Count == 0)
                {
                    if (sb.Length > 0)
                    {
                        list.Add(new JsonProperty(sb.ToString()));
                        sb.Length = 0;
                    }
                }
                else
                {
                    sb.Append(cur);
                }
            }
            if (stack.Count > 0 || stackType.Count > 0)
            {
                list.Clear();
                throw new ArgumentException("无法解析Json Array对象!");
            }
            if (sb.Length > 0)
            {
                list.Add(new JsonProperty(sb.ToString()));
            }
            return list;
        }

        /// <summary>
        ///     Json String解析
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private string ParseString(ref string jsonString)
        {
            var len = jsonString.Length;
            var sb = new StringBuilder();
            var conver = false;
            char cur;
            for (var i = 1; i <= len - 2; i++)
            {
                cur = jsonString[i];
                if (cur == '\\' && !conver)
                {
                    conver = true;
                }
                else if (conver)
                {
                    conver = false;
                    if (cur == '\\' || cur == '\"' || cur == '\'' || cur == '/')
                    {
                        sb.Append(cur);
                    }
                    else
                    {
                        if (cur == 'u')
                        {
                            var temp =
                                new string(new[] { cur, jsonString[i + 1], jsonString[i + 2], jsonString[i + 3] });
                            try
                            {
                                sb.Append((char)Convert.ToInt32(temp, 16));
                            }
                            catch
                            {
                            }
                            i += 4;
                        }
                        else
                        {
                            switch (cur)
                            {
                                case 'b':
                                    sb.Append('\b');
                                    break;

                                case 'f':
                                    sb.Append('\f');
                                    break;

                                case 'n':
                                    sb.Append('\n');
                                    break;

                                case 'r':
                                    sb.Append('\r');
                                    break;

                                case 't':
                                    sb.Append('\t');
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    sb.Append(cur);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        ///     Json Object解析
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private JsonObject ParseObject(ref string jsonString)
        {
            return new JsonObject(jsonString);
        }

        public JsonProperty Add(object value)
        {
            if (Type != JsonPropertyType.Null && Type != JsonPropertyType.Array)
            {
                throw new NotSupportedException("Json属性不是Array类型，无法添加元素!");
            }
            if (_list == null)
            {
                _list = new List<JsonProperty>();
            }
            var jp = new JsonProperty(value);
            _list.Add(jp);
            Type = JsonPropertyType.Array;
            return jp;
        }

        public void Clear()
        {
            Type = JsonPropertyType.Null;
            _value = string.Empty;
            _object = null;
            if (_list != null)
            {
                _list.Clear();
                _list = null;
            }
        }

        public object GetValue()
        {
            if (Type == JsonPropertyType.String)
            {
                return _value;
            }
            if (Type == JsonPropertyType.Object)
            {
                return _object;
            }
            if (Type == JsonPropertyType.Array)
            {
                return _list;
            }
            if (Type == JsonPropertyType.Bool)
            {
                return _bool;
            }
            if (Type == JsonPropertyType.Number)
            {
                return _number;
            }
            return null;
        }

        public virtual T GetValue<T>() where T : class
        {
            return GetValue() as T;
        }

        public virtual void SetValue(object value)
        {
            if (value is string)
            {
                Type = JsonPropertyType.String;
                _value = (string)value;
            }
            else if (value is List<JsonProperty>)
            {
                _list = (List<JsonProperty>)value;
                Type = JsonPropertyType.Array;
            }
            else if (value is JsonObject)
            {
                _object = (JsonObject)value;
                Type = JsonPropertyType.Object;
            }
            else if (value is bool)
            {
                _bool = (bool)value;
                Type = JsonPropertyType.Bool;
            }
            else if (value == null)
            {
                Type = JsonPropertyType.Null;
            }
            else
            {
                double d = 0;
                if (double.TryParse(value.ToString(), out d))
                {
                    _number = d;
                    Type = JsonPropertyType.Number;
                }
                else
                {
                    throw new ArgumentException("错误的参数类型!");
                }
            }
        }

        public override string ToString()
        {
            return ToString("");
        }

        public virtual string ToString(string format)
        {
            var sb = new StringBuilder();
            if (Type == JsonPropertyType.String)
            {
                sb.Append("'").Append(_value).Append("'");
                return sb.ToString();
            }
            if (Type == JsonPropertyType.Bool)
            {
                return _bool ? "true" : "false";
            }
            if (Type == JsonPropertyType.Number)
            {
                return _number.ToString();
            }
            if (Type == JsonPropertyType.Null)
            {
                return "null";
            }
            if (Type == JsonPropertyType.Object)
            {
                return _object.ToString();
            }
            if (_list == null || _list.Count == 0)
            {
                sb.Append("[]");
            }
            else
            {
                sb.Append("[");
                if (_list.Count > 0)
                {
                    foreach (var p in _list)
                    {
                        sb.Append(p);
                        sb.Append(", ");
                    }
                    sb.Length -= 2;
                }
                sb.Append("]");
            }
            return sb.ToString();
        }
    }

    /// <summary>
    ///     GET、POST方式Http请求方法
    /// </summary>
    public class JsonPost
    {
        #region GET、POST方式Http请求方法

        //POST方式发送得结果
        public static string doPostRequest(string url, byte[] bData)
        {
            HttpWebRequest hwRequest;
            HttpWebResponse hwResponse;

            var strResult = string.Empty;
            try
            {
                hwRequest = (HttpWebRequest)WebRequest.Create(url);
                hwRequest.Timeout = 5000;
                hwRequest.Method = "POST";
                hwRequest.ContentType = "application/x-www-form-urlencoded";
                hwRequest.ContentLength = bData.Length;

                var smWrite = hwRequest.GetRequestStream();
                smWrite.Write(bData, 0, bData.Length);
                smWrite.Close();
            }
            catch (Exception err)
            {
                WriteErrLog(err.ToString());
                return strResult;
            }

            //get response
            try
            {
                hwResponse = (HttpWebResponse)hwRequest.GetResponse();
                var srReader = new StreamReader(hwResponse.GetResponseStream(), Encoding.UTF8);
                strResult = srReader.ReadToEnd();
                srReader.Close();
                hwResponse.Close();
            }
            catch (Exception err)
            {
                WriteErrLog(err.ToString());
            }
            return strResult;
        }

        private static void WriteErrLog(string strErr)
        {
            Console.WriteLine(strErr);
            Trace.WriteLine(strErr);
        }

        #endregion GET、POST方式Http请求方法
    }
}