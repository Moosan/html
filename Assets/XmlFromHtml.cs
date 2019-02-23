using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

public class XmlFromHtml
{
    private List<string> ScriptList;
    private List<string> LinkList;
    private List<string> CssList;
    private List<string> InputList;
    private List<string> ImgList;
    private Dictionary<string, List<string>> attributeList;
    private XmlDocument xmlDoc;
    public XmlFromHtml(string html)
    {
        LinkList = new List<string>();
        CssList = new List<string>();
        InputList = new List<string>();
        ImgList = new List<string>();
        ScriptList = new List<string>();
        Parse(html);
    }

    private string HtmlParse(string html) {
        html = html.Replace("<!doctype html>", "").Replace("<!DOCTYPE html>", "");
        Regex htmlRegex = new Regex("(<![^>]+>)|<(([a-z]+)(( |\n)*(([a-z]+)=(\"([^\"]*)\"|([^ ]+) )))*)>");
        Regex scriptEndRegex = new Regex("(</script)(>)");
        html = scriptEndRegex.Replace(html,ScriptSet);
        for (int i = 0; i < scriptCount; i++) {
            Regex iscripttagRegex = new Regex("(<script[^>]*>)(.*)(</script)"+i+">");
        }

        
        return htmlRegex.Replace(html,"");
    }

    public void Parse(string str)
    {
        xmlDoc = new XmlDocument();
        Debug.Log(str);
        string a = str.Replace("<!doctype html>", "").Replace("<!DOCTYPE html>", "").Replace(">", ">\n").Replace("readonly","");

        Debug.Log(a);
        string metareg = "(\n| )*<meta[^>]*>(\n| )*";
        string linkreg = "(\n| )*<link[^>]*>(\n| )*";
        string blankreg = ">(\n| )+";
        string kaigyou = ">\n";
        string noBlank = RegexReplace(blankreg, a, kaigyou);
        string noMeta = RegexReplace(metareg, noBlank, "");
        string noLink = RegexReplace(linkreg, noMeta, "");
        string noHead = RegexReplace("<head(?:(.|\n)+?)?>(.|\n)*?</head>", noLink, "\n");
        string noScript = RegexReplace("<script (?:(.|\n)+?)?>(.|\n)*?</script>", noHead, "\n");
        string noBlank2 = RegexReplace("(\n| )+", noScript, " ");
        string ReplaceAttributeQuote = RegexReplace("([a-z]+)=([^\">'\n;| ().;&]+)( |>)", noBlank2, "$1=\"$2\"$3");
        string seikei = RegexReplace("> <", ReplaceAttributeQuote, ">\n<");
        Regex LinkRemove = new Regex("(href=)(\"[^\"]+\")");
        string hrefChange = LinkRemove.Replace(seikei, LinkSet);
        Debug.Log(hrefChange);
        string noAnd = RegexReplace("&[a-z]+;", hrefChange, "naknanokigou");
        Regex CssRemove = new Regex("(style=)(\"[^\"]+\")");
        string cssChange = CssRemove.Replace(noAnd, CssSet);
        Debug.Log(cssChange);
        string noBr = RegexReplace("<br[^>]*>", cssChange, "");
        string noOnload = RegexReplace("onload=\"[^\"]+\"", noBr, "");
        Regex InputRemove = new Regex("(<input )([^>]+>)");
        string inputChange = InputRemove.Replace(noOnload, InputSet);

        Regex ImgRemove = new Regex("(<img )([^>]+>)");
        string imgChange = ImgRemove.Replace(inputChange, ImgSet);
        
        string noComent = RegexReplace("<![^>]>",imgChange,"");
        string noHatena = RegexReplace("<(.|\n)xml[^>]+>", noComent,"");
        StreamWriter textfile = new StreamWriter("../TextData.txt", false);// TextData.txtというファイルを新規で用意
        textfile.WriteLine(noHatena);// ファイルに書き出したあと改行
        textfile.Flush();// StreamWriterのバッファに書き出し残しがないか確認
        textfile.Close();// ファイルを閉じる
        xmlDoc.Load(new StringReader(noHatena));
    }
    private string LinkSet(Match m)
    {
        LinkList.Add(m.Groups[2].Value);
        return m.Groups[1].Value + "\"" + (LinkList.Count - 1) + "\"";
    }
    private string CssSet(Match m)
    {
        CssList.Add(m.Groups[2].Value);
        return m.Groups[1].Value + "\"" + (CssList.Count - 1) + "\"";
    }
    private string InputSet(Match m)
    {
        InputList.Add(m.Groups[0].Value);
        return m.Groups[1].Value + "value=\"" + (InputList.Count - 1) + "\"/>";
    }
    private string ImgSet(Match m)
    {
        ImgList.Add(m.Groups[0].Value);
        return m.Groups[1].Value + "value=\"" + (ImgList.Count - 1) + "\"/>";
    }
    private static string RegexReplace(string reg, string origin, string replace)
    {
        Regex regex = new Regex(reg);
        string result = regex.Replace(origin, replace);
        Debug.Log(result);
        return result;
    }

    private int scriptCount=0;
    private string ScriptSet(Match m) {
        string result = m.Groups[1].Value + scriptCount + m.Groups[2];
        scriptCount++;
        return result;
    }
    private string SpriptListSet(Match m) {
        ScriptList.Add(m.Groups[2].Value);
        return m.Groups[1].Value + (ScriptList.Count - 1) + m.Groups[3].Value;
    }
    public static void XmlPerse(string xmlString)
    { // string型で渡ってきたとする

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(new StringReader(xmlString));

        XmlNode root = xmlDoc.FirstChild;

        XmlNodeList talkList = xmlDoc.GetElementsByTagName("talk");

        XmlNode talk0 = talkList[0];
        XmlNode talk1 = talkList[1];

        XmlNodeList speakList0 = talk0.ChildNodes;
        XmlNodeList speakList1 = talk1.ChildNodes;

        Debug.Log(root.Name); // talks

        Debug.Log(talk0.Attributes["person"].Value); // 2
        Debug.Log(talk1.Attributes["person"].Value); // 1

        Debug.Log(speakList0[0].Attributes["content"].Value); // こんにちは
        Debug.Log(speakList0[1].Attributes["content"].Value); // ありがとう
        Debug.Log(speakList0[2].Attributes["content"].Value); // さようなら

        Debug.Log(speakList0[0].Attributes["num"].Value); // 1
        Debug.Log(speakList0[1].Attributes["num"].Value); // 2
        Debug.Log(speakList0[2].Attributes["num"].Value); // 3

        Debug.Log(speakList1[0].Attributes["content"].Value); // あーい
        Debug.Log(speakList1[1].Attributes["content"].Value); // ふーい 
        Debug.Log(speakList1[2].Attributes["content"].Value); // ねむい

        Debug.Log(speakList1[0].Attributes["num"].Value); // 4
        Debug.Log(speakList1[1].Attributes["num"].Value); // 5 
        Debug.Log(speakList1[2].Attributes["num"].Value); // 6
    }
    public override string ToString()
    {
        return xmlDoc.ToString();
    }
}