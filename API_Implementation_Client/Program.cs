using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

class Program
{

    //Bu class'ı yazmak sorunda değildik. Ama class tanımlayarak daha güvenli bir yapı oluşturuyorz. Mesela hiç class yazmadan sadece 
    //JSON'u ekrana bastırabiliriz
    //Tabi biz API'ı kullanan olarak server tarafında yani API'ın sahibi student sınıfı diye bir sınıf olduğunu yine API'ın dokümantasyonuna bakarak görebiliriz. Bu durumda en kolay ve profesyonel yöntem Class oluşturmak oldupu için class oluşturduk. API sağlayıcısı,API'ı dokümante etmek zorunda. Eğer dökümantasyon yoksa bile API'ya bir istek atıp, dönen JSON'u manuel inceleyip, ona göre bir class yazabiliriz
    public class Student
    {
        public int Id { get; set; }
        public string firstName { get; set; }
        public string LastName { get;   set; }
        public int Age { get; set; }
       

    }

    //Web'teki herhangi bir server'a bağlanmamız için myHttpClientObject sınıfı kullanırız
    static readonly HttpClient myHttpClientObject = new HttpClient();

    static async Task   GetAllStudent()
    {
        try
        {
            Console.WriteLine("**********Fetching All Students*********");

            //Bu noktada Framework deserializer, JSON key'i ile property ismini karşılaştırıp eşleştiriyor, sırayla değil isimle çalışıyor.
            //Mesela firstname'i alıyor ve JSON'da aynı isimdeki key ile eşleştiriyor. Buna mapping denir. Sonra Age'i age ile diye devam ediyor.
            //JSON'da her bir key mesela bu örnekte id, firstname, lastName ve age { "id": 1, "firstName": "Ahmet", "lastName": "Yılmaz", "age": 20 } bu taraftaki (client side) student sını ile eşleştiriliyor.  büyük-küçük harf'i önemli değil

            //List kullanmamızın sebebi, program mimarisine daha uygun olduğu için bunu kullandık. JSON'dan  deserialize edilelebilen başka bir DS'de kullanabilridik. Tabi tüm DS'lerde olmaz. Sadece JSON'dan deserialize'a uygun olanlar bir diğer tabirle Add() metodu bulunduran DS'lerde olabilir."
            var students = await myHttpClientObject.GetFromJsonAsync<List<Student>>("StudetnsAndMe");

          
            if(students!=null)
            {
                    foreach (var Student in students)
                    {
                        Console.WriteLine($"{Student.firstName} - {Student.LastName} - {Student.Id} - {Student.Age}");
                    }
            }
                
            

        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    //dediğimiz gibi illa class yazmak zorunda değiliz
    static async Task GetAllStudentNoClass()
    {
        try
        {
            Console.WriteLine("**********Fetching All Students*********");

            // Ham JSON'u string olarak al
            var jsonString = await myHttpClientObject.GetStringAsync("RR/StudetnsAndMe");

            //Tüm Json string formatında gelecek. BU string'i istediğin şekilde pars edebilirsin vs. Sonuç olakar illa class olmak zorunda değil.
            //Server bize JSON formatında bilgileri veriyor. Onu nasıl kullanacağın client'in keyfine kalmış. Ha biz yukarda ASP.NET Corê'un bize sağlamış olduğu araçları kullanarak arka planda JSON'un otomatik olarak Students sınıfımızla mapping yapılmasını sağladık. çünkü bu yöntem bizim program algoritma tasarımımız için daha kullanışlı. Eğer JSON formatonda almak  tasarımımız için daha iyi olsa idi öyle yapardık. Tamamen bize kalmış
            Console.WriteLine(jsonString);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
   
    static async Task Main()
    {




        //myHttpClientObject nesnesi için gitmesi gereken URI'i verdik. Bu şekilde hedef URI veriyoruz
        //Burada sadece API'ın genel adresini veririz. Sonra İstedğimiz notkada gereli Endpoint'in URL'lini veririz.
        //Mesela GetAllStudents fonksiyonunda Students'i almak istediğimiz için ilgili endPoint'i verdik.
        //bu URL ve endpoint URL'i önce birleştirilir öyle server'a gider. yani üsteki fonk içinde birleştirilmil link şöyle olur https://localhost:7140/api/RR/StudetnsAndMe
        //Ayrıca URL adresi şu kurala göre bileştir: Sona ekleme şeklinde olmaz. Onun yerine Eğer BaseAddress sonunda / yoksa, base'in son parçası (segment) atılır ve yerine relative path (fonk içinde verdiğimiz string yani sonradan eklenecek olan) konur.
        //bizim örneğimizde BaseAddress = https://localhost:7140/api/ ve Relative path RR/StudentsAndMe. BaseAddress sonu / olduğu için direk ekleme olur ve URL https://localhost:7140/api/RR/StudentsAndMe olur.
        myHttpClientObject.BaseAddress = new Uri("https://localhost:7140/api/");


        await GetAllStudentNoClass();
    }

}
