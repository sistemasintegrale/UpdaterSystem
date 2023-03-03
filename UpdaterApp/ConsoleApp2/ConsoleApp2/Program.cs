// See https://aka.ms/new-console-template for more information


using System.Management;

ManagementObjectSearcher searcher = new
      ManagementObjectSearcher("SELECT * FROM Win32_Processor");
List<string> listProcessor = new List<string>();
foreach (ManagementObject wmi_HD in searcher.Get())
{
    listProcessor.Add(wmi_HD["ProcessorID"].ToString());
    Console.WriteLine(wmi_HD["ProcessorID"].ToString());
}
Console.WriteLine("Hello, World!");
