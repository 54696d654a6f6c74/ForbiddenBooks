using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.ComponentModel;
using ForbiddenBooks.DatabaseLogic.Tables.Base;
using ForbiddenBooks.DatabaseLogic.Tables;
using ForbiddenBooks.DatabaseLogic;

namespace ForbiddenBooks.CLI.Utils
{
    /// <summary>
    /// Reflection based utilities for handling complex method calls.
    /// </summary>
    public abstract class GenericUtil
    {
        /// <summary>
        /// Calls a specifed generic method from a specifed class T with a specified generic type, specifed caller specifed params.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodGenericType"></param>
        /// <param name="methodName"></param>
        /// <param name="caller"></param>
        /// <param name="parameters"></param>
        /// <returns>An object boxed in type T or null depending on the specified function's return type</returns>
        public static object CallGenericMethodFromClass<T>(Type methodGenericType, string methodName, object caller, params object[] parameters) where T : class
        {
            object ret = typeof(T).GetMethod(methodName).MakeGenericMethod(methodGenericType).Invoke(caller, parameters);
            return ret;
        }

        /// <summary>
        /// Prompts the user the input parameters for a specified type T entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dc"></param>
        /// <returns>Returns an entity of type T with matching parameters from the database.</returns>
        public static T FindEntity<T>(DataController dc) where T : class
        {
            DbQuery query = new DbQuery(dc);

            object[] paramS = { };
            string methodName = "";
            string actualType = (typeof(T).ToString()).Split('.').Last();

            while (true)
            {
                if (typeof(T).BaseType == typeof(Item))
                {
                    string name;
                    Console.Write("Name: "); name = Console.ReadLine();

                    paramS = new object[] { name };
                    methodName = $"Get{actualType}sByName";
                }
                else if (typeof(T).BaseType == typeof(Person))
                {
                    string FirstName, LastName;

                    Console.Write("FirstName: "); FirstName = Console.ReadLine();
                    Console.Write("LastName: "); LastName = Console.ReadLine();

                    paramS = new object[] { FirstName, LastName };
                    methodName = $"Get{actualType}sByBothNames";
                }

                object matches = typeof(DbQuery).GetMethod(methodName).Invoke(query, paramS);

                if ((matches as List<T>).Count == 0)
                {
                    Console.WriteLine($"No such {actualType}!");
                    continue;
                }
                
                object copy = Activator.CreateInstance(typeof(T));
                ReflectionCopy((matches as List<T>)[0] as T, copy);

                return copy as T;
            }
        }

        /// <summary>
        /// Copies all properties of Base onto Target.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Base"></param>
        /// <param name="Target"></param>
        public static void ReflectionCopy<T>(T Base, T Target) where T : class
        {
            var baseProps = Base.GetType().GetProperties();
            var targetProps = Target.GetType().GetProperties();

            foreach(var baseProp in baseProps)
            {
                foreach(var tarProp in targetProps)
                {
                    if (baseProp.Name == tarProp.Name && baseProp.PropertyType == tarProp.PropertyType)
                    {
                        tarProp.SetValue(Target, baseProp.GetValue(Base));
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Prompts the user to input values for a new entity of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="change"></param>
        /// <returns>Returns the newly cleated entity of type T.</returns>
        public static T CreateObject<T>(T obj, bool change = false) where T : class
        {
            PropertyInfo[] allProps = obj.GetType().GetProperties();

            foreach (PropertyInfo pinfo in allProps)
            {
                if (pinfo.Name == "Id")
                    continue;

                if (pinfo.PropertyType.IsInterface)
                    continue;

                while (true)
                {
                    Console.Write("New" + pinfo.Name + ": ");
                    string input = Console.ReadLine();

                    if (input == "-1" && change)
                        break;

                    if (pinfo.PropertyType.IsPrimitive ||
                        pinfo.PropertyType == typeof(string) ||
                        pinfo.PropertyType == typeof(decimal))
                    {
                        var converter = TypeDescriptor.GetConverter(pinfo.PropertyType);
                        try
                        {
                            object value = converter.ConvertFromString(input);
                            pinfo.SetValue(obj, value);
                        }
                        catch
                        {
                            Console.WriteLine("Invalid input, please try again...");
                            continue;
                        }
                        break;
                    }
                }
            }
            return obj;
        }

        /// <summary>
        /// Prompts the user the input values for the properties for a new entity of type Magazine.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="dc"></param>
        /// <param name="change"></param>
        /// <returns>Returns the newly created entity of type Magazine</returns>
        public static Magazine CreateMag(Magazine m, DataController dc, bool change = false)
        {
            DbQuery query = new DbQuery(dc);
            string input;

            if (dc.GetEntries<Author>().Count == 0 || dc.GetEntries<Genre>().Count == 0)
            {
                Console.WriteLine("Impossible, there are missing entries");
                return null;
            }

            Console.Write("MagazineName: "); input = Console.ReadLine();
            m.Name = KeepValue(input) ? m.Name : input;

            while (true)
            {
                try
                {
                    Console.Write("MagazinePrice: "); input = Console.ReadLine();
                    m.Price = KeepValue(input) ? m.Price : int.Parse(input);
                }
                catch
                {
                    Console.WriteLine("Invalid input...\n");
                    continue;
                }
                try
                {
                    Console.Write("AccessLevel: "); input = Console.ReadLine();
                    m.AccsessLevel = KeepValue(input) ? m.AccsessLevel : int.Parse(input);
                    break;
                }
                catch
                {
                    Console.WriteLine("Invalid input...\n");
                    continue;
                }
            }

            Author a = m.Author;
            string firstName, lastName;

            while (true)
            {
                Console.Write("Author First Name: "); firstName = Console.ReadLine();
                Console.Write("Author Last Name: "); lastName = Console.ReadLine();

                if ((firstName == "-1" || lastName == "-1") && change)
                    break;

                List<Author> matches = query.GetAuthorsByBothNames(firstName, lastName);

                if (matches.Count == 0)
                {
                    Console.WriteLine("No such author in database");
                    continue;
                }
                else
                {
                    a = matches[0];
                    break;
                }
            }

            Genre g = m.Genre;
            string genreName;

            while (true)
            {
                Console.Write("GenreName: "); genreName = Console.ReadLine();
                if (genreName == "-1" && change) break;

                List<Genre> matches = query.GetGenresByName(genreName);
                if (matches.Count == 0)
                {
                    Console.WriteLine("No such genre in database");
                    continue;
                }
                else
                {
                    g = matches[0];
                    break;
                }
            }

            m.Genre = g;
            m.Author = a;
            return m;

            bool KeepValue(string input)
            {
                if (input == "-1")
                    return true;
                return false;
            }
        }
    }
}
