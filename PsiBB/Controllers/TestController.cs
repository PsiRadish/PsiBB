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
    public class TestController : AsyncController
    {
        public static class Eastman
        {
            public static Random Rando = new Random();
        }

        public abstract class Stateric<T>
        {
            public static int Amazing = Eastman.Rando.Next();
            
            public static string WhatYouSay
            {
                get
                {
                    return typeof(T).Name;
                }
            }
        }
        
        // GET: Test
        public async Task<ActionResult> IndexAsync()
        {
            Random rnd = new Random();
            
            var userRepo = DataAccess.Layer.GetRepository<Models.User>();
            
            // await Task.WhenAll(userRepo.Create(new Models.User { DisplayName="Spartacus", Email=WordFinder2(rnd)+"@"+WordFinder2(rnd)+".net" }),
            //                    userRepo.Create(new Models.User { DisplayName="Spartacus", Email=WordFinder2(rnd)+"@"+WordFinder2(rnd)+".com" }));
            // await Task.WhenAll((new Models.User { DisplayName="Spartacus", Email=WordFinder2(rnd)+"@"+WordFinder2(rnd)+".net" }).Create(),
            //                    (new Models.User { DisplayName="Spartacus", Email=WordFinder2(rnd)+"@"+WordFinder2(rnd)+".com" }).Create());
            
            System.Diagnostics.Debug.Print(Stateric<string>.WhatYouSay);
            System.Diagnostics.Debug.Print(Stateric<Hashtable>.WhatYouSay);
            var amazeString = Stateric<string>.Amazing;
            var amazeHashtable = Stateric<Hashtable>.Amazing;
            System.Diagnostics.Debug.Print(amazeString.ToString(), "Stateric<string>.Amazing from var");
            System.Diagnostics.Debug.Print(Stateric<string>.Amazing.ToString(), "Stateric<string>.Amazing live");
            System.Diagnostics.Debug.Print(amazeHashtable.ToString(), "Stateric<Hashtable>.Amazing from var");
            System.Diagnostics.Debug.Print(Stateric<Hashtable>.Amazing.ToString(), "Stateric<Hashtable>.Amazing live");
            
            //var topicRepo = DataAccess.Layer.GetRepository<Models.Topic>();
            //await topicRepo.Add("somebloodyid", e => e.Posts, new Models.Topic.Post());
            
            // ViewBag.TestOutput = (await userRepo.GetAll()).ToJson(new JsonWriterSettings { Indent = true });
            ViewBag.TestOutput = (await DataAccess.MongoModel<Models.User>.GetAll()).ToJson(new JsonWriterSettings { Indent = true });
            
            return View();
        }
        
        public string WordFinder2(Random rnd)
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

}
