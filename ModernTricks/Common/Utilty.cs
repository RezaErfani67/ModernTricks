using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

public static class Utilty
{
    public enum ImageComperssion
    {
        Maximum = 50,
        Good = 60,
        Normal = 70,
        Fast = 80,
        Minimum = 90,
    }

    public static void ResizeImage(this Stream inputStream, int width, int height, string savePath, ImageComperssion ic = ImageComperssion.Normal)
    {
        System.Drawing.Image img = new Bitmap(inputStream);
        System.Drawing.Image result = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        using (Graphics g = Graphics.FromImage(result))
        {
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.DrawImage(img, 0, 0, width, height);
        }
        result.CompressImage(savePath, ic);
    }

    public static void ResizeImage(this System.Drawing.Image img, int width, int height, string savePath, ImageComperssion ic = ImageComperssion.Normal)
    {
        System.Drawing.Image result = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        using (Graphics g = Graphics.FromImage(result))
        {
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.DrawImage(img, 0, 0, width, height);
        }
        result.CompressImage(savePath, ic);
    }

    public static void ResizeImageByWidth(this Stream inputStream, int width, string savePath, ImageComperssion ic = ImageComperssion.Normal)
    {
        System.Drawing.Image img = new Bitmap(inputStream);
        int height = img.Height * width / img.Width;
        System.Drawing.Image result = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        using (Graphics g = Graphics.FromImage(result))
        {
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.DrawImage(img, 0, 0, width, height);
        }
        result.CompressImage(savePath, ic);
    }

    public static void ResizeImageByWidth(this System.Drawing.Image img, int width, string savePath, ImageComperssion ic = ImageComperssion.Normal)
    {
        int height = img.Height * width / img.Width;
        System.Drawing.Image result = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        using (Graphics g = Graphics.FromImage(result))
        {
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.DrawImage(img, 0, 0, width, height);
        }
        result.CompressImage(savePath, ic);
    }

    public static void ResizeImageByHeight(this Stream inputStream, int height, string savePath, ImageComperssion ic = ImageComperssion.Normal)
    {
        System.Drawing.Image img = new Bitmap(inputStream);
        int width = img.Width * height / img.Height;
        System.Drawing.Image result = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        using (Graphics g = Graphics.FromImage(result))
        {
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.DrawImage(img, 0, 0, width, height);
        }
        result.CompressImage(savePath, ic);
    }

    public static void ResizeImageByHeight(this System.Drawing.Image img, int height, string savePath, ImageComperssion ic = ImageComperssion.Normal)
    {
        int width = img.Width * height / img.Height;
        System.Drawing.Image result = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        using (Graphics g = Graphics.FromImage(result))
        {
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.DrawImage(img, 0, 0, width, height);
        }
        result.CompressImage(savePath, ic);
    }

    public static void CompressImage(this System.Drawing.Image img, string path, ImageComperssion ic)
    {
        System.Drawing.Imaging.EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Convert.ToInt32(ic));
        ImageFormat format = img.RawFormat;
        ImageCodecInfo codec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == format.Guid);
        string mimeType = codec == null ? "image/jpeg" : codec.MimeType;
        ImageCodecInfo jpegCodec = null;
        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
        for (int i = 0; i < codecs.Length; i++)
        {
            if (codecs[i].MimeType == mimeType)
            {
                jpegCodec = codecs[i];
                break;
            }
        }
        EncoderParameters encoderParams = new EncoderParameters(1);
        encoderParams.Param[0] = qualityParam;
        img.Save(path, jpegCodec, encoderParams);
    }

    public static string GetErrors(this ModelStateDictionary modelState)
    {
        return string.Join("<br />", (from item in modelState
                                      where item.Value.Errors.Any()
                                      select item.Value.Errors[0].ErrorMessage).ToList());
    }
    public static string ToLowerFirst(this string str)
    {
        return str.Substring(0, 1).ToLower() + str.Substring(1);
    }

    public static DateTime ToPersianDate(this DateTime dt)
    {
        PersianCalendar pc = new PersianCalendar();
        int year = pc.GetYear(dt);
        int month = pc.GetMonth(dt);
        int day = pc.GetDayOfMonth(dt);
        int hour = pc.GetHour(dt);
        int min = pc.GetMinute(dt);

        return new DateTime(year, month, day, hour, min, 0);
    }

    public static DateTime ToMiladiDate(this DateTime dt)
    {
        PersianCalendar pc = new PersianCalendar();
        return pc.ToDateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0, 0);
    }

    public static string ToPrice(this object dec)
    {
        string Src = dec.ToString();
        Src = Src.Replace(".0000", "");
        if (!Src.Contains("."))
        {
            Src = Src + ".00";
        }
        string[] price = Src.Split('.');
        if (price[1].Length >= 2)
        {
            price[1] = price[1].Substring(0, 2);
            price[1] = price[1].Replace("00", "");
        }

        string Temp = null;
        int i = 0;
        if ((price[0].Length % 3) >= 1)
        {
            Temp = price[0].Substring(0, (price[0].Length % 3));
            for (i = 0; i <= (price[0].Length / 3) - 1; i++)
            {
                Temp += "," + price[0].Substring((price[0].Length % 3) + (i * 3), 3);
            }
        }
        else
        {
            for (i = 0; i <= (price[0].Length / 3) - 1; i++)
            {
                Temp += price[0].Substring((price[0].Length % 3) + (i * 3), 3) + ",";
            }
            Temp = Temp.Substring(0, Temp.Length - 1);
            // Temp = price(0)
            //If price(1).Length > 0 Then
            //    Return price(0) + "." + price(1)
            //End If
        }
        if (price[1].Length > 0)
        {
            return Temp + "." + price[1];
        }
        else
        {
            return Temp;
        }
    }
    public static string Encrypt(this string str)
    {
        byte[] encData_byte = new byte[str.Length];
        encData_byte = System.Text.Encoding.UTF8.GetBytes(str);
        return Convert.ToBase64String(encData_byte);
    }

    public static string Decrypt(this string str)
    {
        System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
        System.Text.Decoder utf8Decode = encoder.GetDecoder();
        byte[] todecode_byte = Convert.FromBase64String(str);
        int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
        char[] decoded_char = new char[charCount];
        utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
        return new string(decoded_char);
    }

    public static string UrlEncode(this string str)
    {
        return HttpUtility.UrlEncode(str);
    }

    public static string UrlDecode(this string str)
    {
        return HttpUtility.UrlDecode(str);
    }


    public static string BeforeDate(this string str)
    {
        DateTime LastDate = DateTime.Parse(str);
        TimeSpan ts = DateTime.Now - LastDate;
        PersianCalendar pc = new PersianCalendar();
        int DifferenceYear = DateTime.Now.Year - LastDate.Year;
        int DiffernceMounth = DateTime.Now.Month - LastDate.Month;
        if (DateTime.Now.Month > LastDate.Month)
            DiffernceMounth = DateTime.Now.Month - LastDate.Month;
        else
            DiffernceMounth = LastDate.Month - DateTime.Now.Month;
        int DifferenceDays = ts.Days;

        StringBuilder Result = new System.Text.StringBuilder("");

        if (DifferenceYear > 0)
        {
            Result.Append(DifferenceYear.ToString() + " سال پیش"/* + " ، " + getDay(pc.GetDayOfWeek(LastDate)) + " " + pc.GetDayOfMonth(LastDate).ToString() + " " + GetMounth(pc.GetMonth(LastDate)) + " " + pc.GetYear(LastDate) + GetHour(LastDate)*/);
        }
        else if (DiffernceMounth > 0)
        {
            Result.Append(DiffernceMounth.ToString() + " ماه پیش" /*+ " ، " + getDay(pc.GetDayOfWeek(LastDate)) + " " + pc.GetDayOfMonth(LastDate).ToString() + " " + GetMounth(pc.GetMonth(LastDate)) + " " + pc.GetYear(LastDate) + GetHour(LastDate)*/);
        }
        else if (DifferenceDays > 0)
            Result.Append(DifferenceDays.ToString() + " روز پیش" /*+ " ، " + getDay(pc.GetDayOfWeek(LastDate)) + " " + pc.GetDayOfMonth(LastDate).ToString() + " " + GetMounth(pc.GetMonth(LastDate)) + " " + pc.GetYear(LastDate) + GetHour(LastDate)*/);
        else if (DifferenceDays == 0)
            Result.Append(" امروز " + /*+ " ، " + getDay(pc.GetDayOfWeek(LastDate)) + " " + pc.GetDayOfMonth(LastDate).ToString() + " " + GetMounth(pc.GetMonth(LastDate)) + " " + pc.GetYear(LastDate) +*/ GetHour(LastDate));

      return Result.ToString();
    }
    public static string getDay(DayOfWeek day)
    {
        string Result = "";
        switch (day)
        {
            case DayOfWeek.Friday:
                Result = "جمعه";
                break;
            case DayOfWeek.Monday:
                Result = "دوشنبه";
                break;
            case DayOfWeek.Saturday:
                Result = "شنبه";
                break;
            case DayOfWeek.Sunday:
                Result = "یکشنبه";
                break;
            case DayOfWeek.Thursday:
                Result = "پنج شنبه";
                break;
            case DayOfWeek.Tuesday:
                Result = "سه شنبه";

                break;
            case DayOfWeek.Wednesday:
                Result = "چهارشنبه";
                break;
            default:
                break;
        }
        return Result;
    }

    public static string GetHour(DateTime lastdate)
    {
        PersianCalendar pc = new PersianCalendar();
        string result = " ساعت " + (((pc.GetHour(lastdate)) < 10) ? ("0" + pc.GetHour(lastdate).ToString()) : (pc.GetHour(lastdate)).ToString()) + ":" + (((pc.GetMinute(lastdate)) < 10) ? ("0" + pc.GetMinute(lastdate).ToString()) : (pc.GetMinute(lastdate)).ToString());
        return result;
    }
    public static string GetMounth(int month)
    {
        string[] monthInYear = { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };
        return monthInYear[month - 1];
    }

    public static ExpandoObject ToExpando(this object anonymousObject)
    {
        IDictionary<string, object> anonymousDictionary = new RouteValueDictionary(anonymousObject);
        IDictionary<string, object> expando = new ExpandoObject();
        foreach (var item in anonymousDictionary)
            expando.Add(item);
        return (ExpandoObject)expando;
    }
}
