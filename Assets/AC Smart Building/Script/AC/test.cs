using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace AC
{
    class Program
    {
        static void Main(string[] args)
        {
            var tList = new TList();
            tList.t1 = new List<T1>() { new T1("Ali"), new T1("Ehsan"), new T1("Morteza") };
            tList.t2 = new List<T2>() { new T2("Maryam"), new T2("Zahra"), new T2("Zohreh") };
            tList.t3 = new List<T3>() { new T3("Fatemeh"), new T3("Jafar"), new T3("Sagha") };
            //move in multi
            foreach (PropertyInfo property in tList.GetType().GetProperties())
            {
                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    IList list = (IList)property.GetValue(tList);
                    foreach (var item in list)
                    {
                        // Perform the operation on each item
                        Console.WriteLine(item);
                    }
                }
            }
        }
    }

    public class T1
    {
        public string name { get; set; }

        public T1(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return $"{name}";
        }
    }

    public class T2
    {
        public string name { get; set; }

        public T2(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return $"{name}";
        }
    }

    public class T3
    {
        public string name { get; set; }

        public T3(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return $"{name}";
        }
    }

    public class TList
    {
        public List<T1> t1 { get; set; }
        public List<T2> t2 { get; set; }
        public List<T3> t3 { get; set; }
    }
}