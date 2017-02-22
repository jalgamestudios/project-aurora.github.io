using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace markdownToHTML
{
    class Program
    {
        static bool useStylesheet = false;
        static string stylesheetName = "";
        static bool isCode = false;
        static string outputName = "C:\\Users\\Luis\\Desktop\\example\\example1.html";
        static string inputName = "C:\\Users\\Luis\\Documents\\GitHub\\project-berserker\\documentation\\klausur-one\\documentation.md";
        

        static void Main(string[] args)
        {
            


            //Main Code
            List<string> markdownInput = getInputText();
            List<string> htmlOutput = new List<string>();
            htmlOutput = convertToHTML(markdownInput);
            printStringList(htmlOutput);
            printToFile(htmlOutput);

            //Not closing console:
            Console.ReadLine();

        }
        static List<string> convertToHTML(List<string> input)
        {
            List<string> output = new List<string>();
            //List of activated identifieres sorted by priority:
            List<string> identifiers = new List<string>();
            // standard html stuff
            output.Add("<!DOCTYPE html> <html> <head>");
            output.Add("<title>" + findTitle(input) + "</title>");
            if (useStylesheet)
            {
                output.Add("<link rel = \"stylesheet\" href = "+stylesheetName+" >");
            }
            // add more <head> stuff here:

           



            output.Add("</head> <body>");



            // main routine
            input = shortenList(input);
            for (int i = 0; i < input.Count; i++)
            {
                string newline = replaceLine(input[i]);
                if (newline.StartsWith("<li>"))
                {
                    if (i!=0&& i != input.Count &&replaceLine(input[i-1]).StartsWith("*")&& replaceLine(input[i + 1]).StartsWith("*"))
                    {
                        output.Add(newline.Replace("*", ""));
                    }
                    else if (i == 0 || !replaceLine(input[i - 1]).StartsWith("<li>"))
                    {
                        output.Add("<ul>" + (newline.Replace("*", "")));
                    }
                    else
                    {
                        output.Add((newline.Replace("*", "") + "</ul>"));
                    }
                }
                else if (newline != "")
                {
                    output.Add(newline);
                }
                input[i] = input[i].Replace("\\","");
            }




            output.Add("</body> </html>");
            for (int i = 0; i < output.Count; i++)
            {
                output[i].Replace("\\","");
            }
            return output;
        }
        static string replaceLine(string input)
        {
            input.Trim();
            
            int a = 0;
            if (input.StartsWith("#"))
            {
                //is title
                int titleValue = determineHeaderValue(input);
                return "<h" + titleValue.ToString() + ">" + input.Replace("#","") + "</h" + titleValue.ToString() + ">";
            }
            else if (input.StartsWith("*"))
            {
                //is List
                return "<li>" + input + "</li>";
            }
            else if (input.StartsWith("!"))
            {
                //is picture
                return "<img src =\"" +getLink(input) + "\" alt=\""+getAlt(input)+"\" style=\"width:width;height:height\" title=\""+ getTitle(input) + "\"/>";
            }
            else if (input != "")
            {
                //is paragarph
                return "<p>" + input + "</p>";
            }
            else
            {
                return "";
            }


        }
        static string getRefLink(string input)
        {
            string[] outputArr = input.Split('[');
            string[] outputArr2 = input.Split('#');
            string[] outputArr3 = outputArr2[1].Split(')');
            return outputArr[0] + "<a href=\""+outputArr3[0]+"\">"+getAlt(input)+"</a>";
        }
        static string getTitle(string input)
        {
            string[] outputArr = input.Split('"');
            return outputArr[1];
        }
        static string getAlt(string input)
        {
            string[] outputArr = input.Split('[');
            string[] outputArr2 = outputArr[1].Split(']');
            return outputArr2[0];
        }
        static string getLink(string input)
        {
            string[] outputArr = input.Split('(');
            string[] outputArr2 = outputArr[1].Split(' ');
            return outputArr2[0];
        }
        static int determineHeaderValue(string input)
        {
            if (input.StartsWith("######"))
            {
                return 6;
            }
            else if (input.StartsWith("#####"))
            {
                return 5;
            }
            else if (input.StartsWith("####"))
            {
                return 4;
            }
            else if (input.StartsWith("###"))
            {
                return 3;
            }
            else if (input.StartsWith("##"))
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }
        static List<string> shortenList(List<string> input)
        {
            for (int i = 0; i < input.Count; i++)
            {
                if (!notSpecialCase(input, i))
                {
                    if (input[i].StartsWith(" "))
                    {
                        input[i - 1] = input[i - 1] + input[i];
                    }
                    else
                    {
                        input[i - 1] = input[i - 1] + " " + input[i];
                    }
                    input.RemoveAt(i);
                    i = 0;


                }
            }
            return input;
        } 
        static bool notSpecialCase(List<string> input, int i)
        {
            int useless = 0;
            if (input[i] != "" && i != 0 &&input[i-1] !=""&& !input[i - 1].StartsWith("#") && !input[i - 1].StartsWith("!") && !input[i - 1].StartsWith("`") && !input[i - 1].StartsWith("*")&& !Int32.TryParse(input[i][0].ToString(), out useless))
            {
                return false;
            }
            return true;
        }
        static string findTitle(List<string> input)
        {
            
            for (int i = 0; i < input.Count; i++)
            {
                if (input[i].StartsWith("#"))
                {
                    return (input[i].Replace("#", ""));
                }
            }
            return "missing Title";
        }
        static List<string> getInputText()
        {
            List<string> markdownInput = new List<string>();
            StreamReader sr = new StreamReader(inputName);
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                markdownInput.Add(line);
            }
            sr.Close();
            return markdownInput;
        }
        static void printStringList(List<string> input)
        {
            for (int i = 0; i < input.Count; i++)
            {
                Console.WriteLine(i);
                Console.WriteLine(input[i]);
            }
        }
        static string stringListToString(List<string> input)
        {
            string output = "";
            for (int i = 0; i < input.Count; i++)
            {
                output = output + System.Environment.NewLine + input[i];

            }
            return output;
        }
        static void printToFile(List<string> input)
        {
            StreamWriter sw = new StreamWriter(outputName);
            for (int i = 0; i < input.Count; i++)
            {
                sw.WriteLine(input[i]);
            }
            sw.Close();
        }
    }
}
