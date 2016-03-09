using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using System.Collections;

namespace PsiBB.Controllers
{
    public abstract class Toolbox
    {
        public static string WordFinder2(Random rnd)
        {
            int wordLength = rnd.Next(3, 7);
            
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z" };
            string[] vowels = { "a", "e", "i", "o", "u" };
            
            string word = "";
            
            if (wordLength == 1)
            {
                word = GetRandomLetter(rnd, vowels);
            }
            else
            {
                for (int i = 0; i < wordLength; i += 2)
                {
                    word += GetRandomLetter(rnd, consonants) + GetRandomLetter(rnd, vowels);
                }
                
                word = word.Replace("q", "qu").Substring(0, wordLength); // We may generate a string longer than requested length, but it doesn't matter if cut off the excess.
            }
            
            return word;
        }
        
        private static string GetRandomLetter(Random rnd, string[] letters)
        {
            return letters[rnd.Next(0, letters.Length - 1)];
        }
    }
    
    // public static class Eastman
    // {
    //     public static Random Rando = new Random();
    // }
    
    // public abstract class Stateric<T>
    // {
    //     public static int Amazing = Eastman.Rando.Next();
        
    //     public static string WhatYouSay
    //     {
    //         get
    //         {
    //             return typeof(T).Name;
    //         }
    //     }
    // }
    
    public class TestController : AsyncController
    {
        // GET: Test
        public async Task<ActionResult> IndexAsync()
        {
            // var userRepo = DataAccess.Layer.GetRepository<Models.User>();
            
            // Random rnd = new Random();
            // await Task.WhenAll(userRepo.Create(new Models.User { DisplayName="Spartacus", Email=Toolbox.WordFinder2(rnd)+"@"+Toolbox.WordFinder2(rnd)+".net" }),
            //                     userRepo.Create(new Models.User { DisplayName="Spartacus", Email=Toolbox.WordFinder2(rnd)+"@"+Toolbox.WordFinder2(rnd)+".com" }));
            // await Task.WhenAll((new Models.User { DisplayName="Spartacus", Email=Toolbox.WordFinder2(rnd)+"@"+Toolbox.WordFinder2(rnd)+".net" }).Create(),
            //                    (new Models.User { DisplayName="Spartacus", Email=Toolbox.WordFinder2(rnd)+"@"+Toolbox.WordFinder2(rnd)+".com" }).Create());
            
            // ViewBag.TestOutput = (await userRepo.GetAll()).ToJson(new JsonWriterSettings { Indent = true });
            // ViewBag.TestOutput = (await DataAccess.MongoModel<Models.User>.GetAll()).ToJson(new JsonWriterSettings { Indent = true });
            
            // System.Diagnostics.Debug.Print(Stateric<string>.WhatYouSay);
            // System.Diagnostics.Debug.Print(Stateric<Hashtable>.WhatYouSay);
            // var amazeString = Stateric<string>.Amazing;
            // var amazeHashtable = Stateric<Hashtable>.Amazing;
            // System.Diagnostics.Debug.Print(amazeString.ToString(), "Stateric<string>.Amazing from var");
            // System.Diagnostics.Debug.Print(Stateric<string>.Amazing.ToString(), "Stateric<string>.Amazing live");
            // System.Diagnostics.Debug.Print(amazeHashtable.ToString(), "Stateric<Hashtable>.Amazing from var");
            // System.Diagnostics.Debug.Print(Stateric<Hashtable>.Amazing.ToString(), "Stateric<Hashtable>.Amazing live");
            
            //var topicRepo = DataAccess.Layer.GetRepository<Models.Topic>();
            //await topicRepo.Add("somebloodyid", e => e.Posts, new Models.Topic.Post());
            
            var topic = new Models.Topic();
            var postsProp = (typeof (Models.Topic)).GetProperty("Posts");
            var testProp = (typeof(Models.Topic)).GetProperty("TEST");

            // var mongoListElementType = typeof(List<DataAccess.MongoListElement>).GetGenericArguments()[0];
            // var mongoListElementType = typeof(DataAccess.MongoListElement);
            // var postsElementType = postsProp.PropertyType.GetGenericArguments()[0];

            //IEnumerable<DataAccess.MongoListElement> list = topic.Posts;

            ViewBag.TestOutput = String.Format("{0}   {1}", typeof(IEnumerable<DataAccess.MongoListElement>).IsAssignableFrom(postsProp.PropertyType), typeof(IEnumerable<DataAccess.MongoListElement>).IsAssignableFrom(testProp.PropertyType));
            //ViewBag.TestOutput = String.Join(", ", postsProp.PropertyType.GetInterfaces().Select(x => x.ToString()).ToArray()) + '\n' + typeof(IList<DataAccess.MongoListElement>).ToString();
            //ViewBag.TestOutput = String.Format("{0}   {1}   {2}", mongoListElementType, postsElementType, postsElementType.IsSubclassOf(mongoListElementType));
            
            return View();
        }
    }
}
