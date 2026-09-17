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
        public double Grade { get; set; }

        public Student()
        {


        }

        public Student(int ID,int age,double grade,string firstName,string lastName)
        {
            this.firstName = firstName;
            this.Id = ID;
            this.Grade = grade;
            this.Age = age;
            this.LastName = lastName;
        }

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
            //JSON'da her bir key mesela bu örnekte id, firstname, lastName ve age { "id": 1, "firstName": "Ahmet", "lastName": "Yılmaz", "age": 20 } bu taraftaki (client side) st udent sını ile eşleştiriliyor.  büyük-küçük harf'i önemli değil

            //List kullanmamızın sebebi, program mimarisine daha uygun olduğu için bunu kullandık. JSON'dan  deserialize edilelebilen başka bir DS'de kullanabilridik. Tabi tüm DS'lerde olmaz. Sadece JSON'dan deserialize'a uygun olanlar bir diğer tabirle Add() metodu bulunduran DS'lerde olabilir."
            var students = await myHttpClientObject.GetFromJsonAsync<List<Student>>("all");

          
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
            //Server side'a StudentList fonsiyonun ENdpoint URL'li https://localhost:7140/api/Students olduğu ve bunu zaten main'de verildiği için bu parantez içinde sadece all dedemiz yeterli. Anca o zaman URL/endpoint doğru olur
            var jsonString = await myHttpClientObject.GetStringAsync("all");

            //Tüm Json string formatında gelecek. BU string'i istediğin şekilde pars edebilirsin vs. Sonuç olakar illa class olmak zorunda değil.
            //Server bize JSON formatında bilgileri veriyor. Onu nasıl kullanacağın client'in keyfine kalmış. Ha biz yukarda ASP.NET Corê'un bize sağlamış olduğu araçları kullanarak arka planda JSON'un otomatik olarak Students sınıfımızla mapping yapılmasını sağladık. çünkü bu yöntem bizim program algoritma tasarımımız için daha kullanışlı. Eğer JSON formatonda almak  tasarımımız için daha iyi olsa idi öyle yapardık. Tamamen bize kalmış
            Console.WriteLine(jsonString);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
   

    static async Task GetPassedStudents()
    {
        try
        {
            Console.WriteLine("**********Fetching All passed  Students*********");


            var students = await myHttpClientObject.GetFromJsonAsync<List<Student>>("Passed");


            if (students != null)
            {
                foreach (var Student in students)
                {
                    Console.WriteLine($"{Student.firstName} - {Student.LastName} - ID: {Student.Id} - Age: {Student.Age} - Grade: {Student.Grade}");
                }
            }



        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    static async Task GetAverageGrade()
    {
        try
        {
            Console.WriteLine("**********Fetching Avr. Grade*********");

            //GetFromJsonAsync - GetFromJsonAsync fonksiyonuda veridimiz değişken türü aslında gelen JSON'un hangi türe dönüştürülmesini söylüyor. yani **`GetFromJsonAsync<T>`'deki `T`, "gelen JSON'u hangi C# tipine dönüştür" talimatıdır** — framework'e "bu JSON'u şu kalıba (tipe) sok" diyorsun. Mesela List<Studnt> olduğunda framework bizim gelen JSON verisin Stundet türünden liste'e çevirmemiz gerektiğini düşünüyor ve ona çeviriypr. Eğer double deseydik bu sefer JSON'dan gelen veriyi double'a çevirirdi. Tabi gelen verini ne olduğunu göz önünde bulundurmalıyız. yani 
            //Framework, **"bu JSON gerçekten bu tipe uyar mı" * *diye önceden kontrol etmez, sen `T` (değişken türü) olarak ne yazarsan, **o kalıba zorlamaya çalışır.* *Uyumsuzsa, hata(`JsonException`) çalışma zamanında(**runtime * *) patlar.Gidipte JSON'da birden fazla satırdan oluşan veriyi tek bir double değişkenine dönüştürmeye çalışmamalıyız. Ancak tek bir değer geliyorsa o zaman sıkıntı yok. mesela `double avr = await myHttpClientObject.GetFromJsonAsync<double>("Avr");`
            //- Örneğin `double avr = await GetFromJsonAsync<double>("Students");  // ❌ hata!`

            //Burada** çalışma zamanında * * `JsonException` alırsın, çünkü bir JSON array'i tek bir `double`'a sığdırmaya çalışıyorsun — framework bunu **önceden bilemez * *, sen `T`'yi yanlış seçtiğin an çalıştırıp görürsün.
            //
            var avr = await myHttpClientObject.GetFromJsonAsync<double>("AverageGrade");
                
            if(avr!=null)
            {
                Console.WriteLine("Average Grade is : " + avr);
            }
              
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    static async  Task GetStudentByID(int ID)
    {
        try
        {
            Console.WriteLine("**********Fetching Avr. Grade*********");

            var avr = await myHttpClientObject.GetFromJsonAsync<Student>($"{ID}");

            if (avr != null)
            {
                Console.WriteLine("Average Grade is : " + avr.firstName);
               
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    //3 farklı dönüş tipi olabilir. Bu nedenli client side'a yani burada bunları daha net görebilemk için bu sefer status code'a göre çıktı verebilriz
    static async Task GetStudentById(int id)
    {
        try
        {
            Console.WriteLine("\n_____________________________");
            Console.WriteLine($"\nFetching student with ID {id}...\n");

            //ilgili Endpoint'a gidip responsu alır.
            var response = await myHttpClientObject.GetAsync($"{id}");

            //1.kutuda eğer successs kodu ise if içinden devam ederiz
            if (response.IsSuccessStatusCode)
            {
                //Şimdi burad tekrardan server'a gitmeyiz. Server'a zaten yukardaki kodda gittik. burada gelen JSON'u ReadFromJsonAsync fonksiyonu ile Student objesine dönüştürürüz
                var student = await response.Content.ReadFromJsonAsync<Student>();
                //Her türlü null dönme ihtimaine karşı kontrol ederiz
                if (student != null)
                {
                    Console.WriteLine($"ID: {student.Id}, Name: {student.firstName}, Age: {student.Age}, Grade: {student.Grade}");
                }
            }
            //Eğer 1.Kutuda badrequest varsa ekrana bu mesajı basarız
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                Console.WriteLine($"Bad Request: Not accepted ID {id}");
            }
            //Eğer 1.Kutuda NotFound varsa ekrana bu mesajı basarız
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Not Found: Student with ID {id} not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static async Task AddNewStudent(Student student)
    {
        try
        {
            Console.WriteLine("\n_____________________________");
            Console.WriteLine("\nAdding a new student...\n");


            //response değişkeni(HttpResponseMessage tipi),  3 parçayı birlikte taşıyor:
            /*
             1-Response body (yeni student verisi burada), 2-header (URL burada) ve 3-Status code (201 created)
             
            Ayırca tüm kısımlar JSON değil. sadece body JSON. Header ve status code düz text'ti ve HTTP'in sorumluluğundadır.
             */
            var response = await myHttpClientObject.PostAsJsonAsync("",student);

            if(response.IsSuccessStatusCode)
            {
                //eğer gelen respıns başarılı ise yani gerçekten veri varsa gelen response'ran body'i yani JSON'u okuyoruz. Kısacası veriyi alıyoruz.
                var addedStudent = await response.Content.ReadFromJsonAsync<Student>();
                Console.WriteLine($"Added Student - ID: {addedStudent.Id}, Name: {addedStudent.firstName}, Age: {addedStudent.Age}, Grade: {addedStudent.Grade}");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                Console.WriteLine("Bad Request: Invalid student data.");
            }





        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

    }


    static async Task DeleteStudent(int StudentID)
    {
        try
        {
            Console.WriteLine("\n_____________________________");
            Console.WriteLine("\n Delete student\n");



            var response = await myHttpClientObject.DeleteAsync($"{StudentID}");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Student Deleted Successfully");
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                Console.WriteLine("Bad Request: Invalid student data.");
            }
            else if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Not Found: Student with ID {StudentID} could not be found.");

            }
            else
            {
                Console.WriteLine("Un");
            }





        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

    }



    static async Task Main()    
    {




        /*myHttpClientObject nesnesi için gitmesi gereken URI'i verdik. Bu şekilde hedef URI veriyoruz
        //Burada sadece API'ın genel adresini veririz. Sonra İstedğimiz notkada gereli Endpoint'in URL'lini veririz.
        //Mesela GetAllStudents fonksiyonunda Students'i almak istediğimiz için ilgili endPoint'i verdik.
        //bu URL ve endpoint URL'i önce birleştirilir öyle server'a gider. yani üsteki fonk içinde birleştirilmil link şöyle olur https://localhost:7140/api/RR/StudetnsAndMe
        Ayrıca URL adresi şu kurala göre bileştir: Sona ekleme şeklinde olmaz. Onun yerine Eğer BaseAddress sonunda / yoksa, base'in son parçası (segment) atılır ve yerine relative path (fonk içinde verdiğimiz string yani sonradan eklenecek olan) konur.
        bizim örneğimizde BaseAddress = https://localhost:7140/api/ ve Relative path RR/StudentsAndMe. BaseAddress sonu / olduğu için direk ekleme olur ve URL https://localhost:7140/api/RR/StudentsAndMe olur. Yani günü sonunda server'a giden URL swagger'de ki gibi olmalı. Nerede oluşturduğu pek öneli değil. istersen buradan direk tüm URL'e ver, istersen sadece Students'e kadar olan kısmı ve geri kalamın istediğin yerde yap
       ;
        */
        myHttpClientObject.BaseAddress = new Uri("https://localhost:7140/api/Students/");

        await GetAllStudent();

    await    DeleteStudent(1);


       await GetAllStudent();
    }

}
