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
    /*public abstract class Toolbox
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
    }*/
    
    /*public static class Eastman
    {
        public static Random Rando = new Random();
    }*/
    
    /*public abstract class Stateric<T>
    {
        public static int Amazing = Eastman.Rando.Next();
        
        public static string WhatYouSay
        {
            get
            {
                return typeof(T).Name;
            }
        }
    }*/
    
    public class TestController : AsyncController
    {
        // GET: Test
        public async Task<ActionResult> IndexAsync()
        {
            // Random rnd = new Random();
            // await Task.WhenAll((new Models.User { DisplayName="Spartacus", Email=Toolbox.WordFinder2(rnd)+"@"+Toolbox.WordFinder2(rnd)+".net" }).CreateAsync(),
            //                    (new Models.User { DisplayName="Spartacus", Email=Toolbox.WordFinder2(rnd)+"@"+Toolbox.WordFinder2(rnd)+".com" }).CreateAsync());
            
            // ViewBag.TestOutput = (await Models.User.GetAllAsync()).ToJson(new JsonWriterSettings { Indent = true });
            
            var nonyNony = new { Nummer = 10, Strang = "Fluffy" };
            ViewBag.TestOutput = nonyNony.GetType().ToString();
            
            // var topic = new Models.Topic();
            // var postsProp = (typeof (Models.Topic)).GetProperty("Posts");
            // var testProp = (typeof(Models.Topic)).GetProperty("TEST");
            
            // ViewBag.TestOutput = String.Format("{0}   {1}", typeof(IEnumerable<DataAccess.MongoListElement>).IsAssignableFrom(postsProp.PropertyType), typeof(IEnumerable<DataAccess.MongoListElement>).IsAssignableFrom(testProp.PropertyType));
            
            return View();
        }
    }
}
