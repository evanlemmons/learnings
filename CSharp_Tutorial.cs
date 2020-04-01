/// <summary> *************************************************************
/// DELEGATES, EVENTS, AND LAMBDAS
/// </summary> *************************************************************

//EVENTS AND DELEGATES
//https://learn.unity.com/tutorial/challenge-teleport-events?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5cf069dfedbc2a5adef4544d#5cf06507edbc2a57ca24d12c

using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CSharp_Tutorial //namespacing so other code isn't effected by this
{
    //Main.cs
    public class MainTutorial : MonoBehaviour
    {
        public delegate void Teleport(Vector3 pos); //gotta define a delegate to be used by your event
        public static event Teleport OnTeleport; //the event of type UseSpaceBar delegate ☝️

        private Vector3 _teleportPosition = new Vector3(5, 2, 0);

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) //some trigger to fire off your event
            {
                if (OnTeleport != null)
                    OnTeleport(_teleportPosition);
            }
        }
    }

    //Cube.cs
    public class Cube : MonoBehaviour

    {
        private Vector3 teleportPosition = new Vector3(5, 2, 0);

        private void Start()
        {
            MainTutorial.OnTeleport += Teleport; //assigns Teleport() to Main.HitSpace
        }

        private void Teleport(Vector3 pos) //the function to be assigned
        {
            transform.position = pos; //the act of teleporting the object to the teleport position
        }

        private void OnDisable()
        {
            MainTutorial.OnTeleport -= Teleport;
        }
    }

    //PRACTICAL EVENT DRIVEN PROGRAMMING
    //https://learn.unity.com/tutorial/practical-event-driven-programming?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5cf069dfedbc2a5adef4544d#5cf0656eedbc2a5aba41a984

    //player, UI manager, game manager
    //hit space = player dies, increment the death count, update the UI
    //instead of notifying the game manager and the UI know when the player dies,
    //create an event that anyone who is INTERESTED in knowing the player died, know

    //Player.cs
    public class Player : MonoBehaviour
    {
        public delegate void OnDeath();
        public static event OnDeath PlayerDead;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) //see if anyone cares that you're dead
            {
                Death(); //call the death method which triggers an event to all the subscribers
            }
        }

        void Death() //the event that lets all subscribers know u ded
        {
            PlayerDead?.Invoke(); //the exact same thing as saying "if(PlayerDead != null) { PlayerDead }"
        }
    }

    //GameManager.cs
    public class GameManager : MonoBehaviour
    {
        private void OnEnable()
        {
            Player.PlayerDead += ResetPlayer; //when the player dies, reset their stats and prep for respawn
        }

        public void ResetPlayer() //what actually happens on Player.PlayerDead
        {
            Debug.Log("Resetting Player Info");
        }
    }

    //UIManager.cs
    public class UIManager : MonoBehaviour
    {
        public int deathCount;
        public Text deathCountText;

        public void OnEnable()
        {
            Player.PlayerDead += UpdateDeathCount; //now both the UIManager and the GameManager are subscribed to the Player's death event
        }

        public void UpdateDeathCount() //they can both do things independently of each other and the Player doesn't have to care
        {
            deathCount++;
            deathCountText.text = "Death Count: " + deathCount;
        }
    }

    //ACTIONS
    //Pretty much like delegates and events, but combined into a single line instead of two
    //https://learn.unity.com/tutorial/c-actions?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5cf069dfedbc2a5adef4544d
    //In this example, an action is triggered when the player takes damage. Notifies the UI and game managers.
    //the first bit is using events, the second bit shows how actions can clean it up

    public class Player_UsingEvents : MonoBehaviour
    {
        public delegate void OnDamageReceived(int currentHealth); //anyone who wants to know when the player is damage and what the remaining health is after being damaged can subscribe to this
        public static event OnDamageReceived OnDamage; //this is the actual event they're going to subscribe to

        public int Health { get; set; }

        private void Start()
        {
            Health = 10;
        }

        void Damage()
        {
            Health--;
            OnDamage?.Invoke(Health); // same as saying "if(OnDamage != null) { OnDamage(Health) }
        }
    }

    public class UIManager_UsingEvents : MonoBehaviour
    {
        private void OnEnable()
        {
            Player_UsingEvents.OnDamage += UpdateHealth; //don't have to include () after UpdateHealth because the signature is inferred in the declaration below.
        }

        public void UpdateHealth(int health)
        {
            //display updated health in the UI
        }
    }

//☝️ Same code but using delegates/events
//👇 Same code but using actions

    using System; //gotta have this to use actions

public class Player_UsingActions : MonoBehaviour
    {
        //public delegate void OnDamageReceived(int currentHealth); the old way
        //public static event OnDamageReceived OnDamage; the old way


        public Action onComplete; //identical to saying "public delegate void onComplete();"

        public static Action<int> OnDamageReceived; //identical to the two lines of code above defining OnDamage and OnDamageReceived(int currentHealth)


        public int Health { get; set; }

        private void Start()
        {
            Health = 10;
        }

        void Damage()
        {
            Health--;
            OnDamageReceived?.Invoke(Health); // same as saying "if(OnDamage != null) { OnDamage(Health) }
        }
    }

    public class UIManager_UsingActions : MonoBehaviour
    {
        private void OnEnable()
        {
            Player_UsingActions.OnDamageReceived += UpdateHealth; //don't have to include () after UpdateHealth because the signature is inferred in the declaration below.
        }

        public void UpdateHealth(int health)
        {
            //display updated health in the UI
        }
    }

    //RETURN TYPE DELEGATES USING FUNC // this one makes no freaking sense at all
    //https://learn.unity.com/tutorial/c-return-type-delegates-and-func?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5cf069dfedbc2a5adef4544d#5cf0674dedbc2a5847867fe7
    //Functional delegates. Return type delegates.

    public class ReturnTypeDelegates_Ex1 : MonoBehaviour
    {
        public delegate void OnSomeAction(); // returns nothing, acts as a callback system
        public delegate string OnAnotherAction(); //returns a string to be used

        //👇 Create a program that takes in a string and returns the length of it.

        int GetCharacters(string name) //the method that actually counts the characters of a string
        {
            return name.Length;
        }

        private void Start()
        {
            string name = "Evan";
            int characterCount = GetCharacters(name);
            Debug.Log("Character Count: " + characterCount);
        }
    }

    //👇 Now do that same thing but using a return type delegate

    public class ReturnTypeDelegates_Ex2 : MonoBehaviour
    {
        public delegate int CharacterLength(string text);

        private void Start()
        {
            CharacterLength cl = new CharacterLength(GetCharacters); //this assigns the GetCharacters function to the return type delegate variable "cl"

            Debug.Log(cl("Evan")); //
        }

        int GetCharacters(string name) //the method that actually counts the characters of a string
        {
            return name.Length;
        }
    }

    // So I totally don't get the purpose of ☝️ just yet
    // But 👇 is going to expound on it further 🤦‍♂️

    public class ReturnTypeDelegates_Ex3 : MonoBehaviour
    {
        public Func<string, int> CharacterLength; //the same as above: a return type delegate which accepts a string and returns an int

        private void Start()
        {
            CharacterLength = GetCharacters; //but hold up, you just assign it to a variable and don't use += ??? how is it a delegate then?

            int count = CharacterLength("Jehoshaphat");

            Debug.Log("Count: " + count);
        }

        int GetCharacters(string name) //the method that actually counts the characters of a string
        {
            return name.Length;
        }
    }

    //LAMBDA EXPRESSIONS
    //https://learn.unity.com/tutorial/c-lambda-expressions?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5cf069dfedbc2a5adef4544d
    //If the last one made no sense, I'm expecting this to be more of the same
    //Lambda expressions help you make the most of using delegates
    //Essentially, they allow you to write methods inline

    public class LambdaExpressions_Ex1 : MonoBehaviour
    {
        public Func<string, int> CharacterLength;

        private void Start()
        {
            CharacterLength = (name) => name.Length; //identical to the GetCharacters method below
                                                     // => is the Lambda operator which means "go to" so (thing in here) => go to (thing over here)
                                                     //the code is inferring a lot of things based on how you set up the delegate above
                                                     //it knows (name) will be a string and that .Length will return an int which is what CharacterLength is looking for

            int count = CharacterLength("Jehoshaphat");

            Debug.Log("Count: " + count);
        }

        //Lambdas allow for creating methods inline, removing the need for them elsewhere in the script
        //int GetCharacters(string name)
        //{
        //    return name.Length;
        //}
    }

    //PRACTICE W/ DELEGATES
    //https://learn.unity.com/tutorial/practicing-c-delegates-with-and-without-return-types-and-parameters?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5cf069dfedbc2a5adef4544d

    //1ST VIDEO
    //Create a delegate of type void that calculates the sum of two numbers. Use a lambda to avoid having a dedicated method.
    //You can open curly braces after a lambda => to write some quick, basic functions without having to create specific methods for them
    public class DelegateChallenege1 : MonoBehaviour
    {
        public Action<int, int> Sum; //return type void is understood

        private void Start()
        {
            Sum = (a, b) =>
            {
                var total = a + b;
                Debug.Log("Total: " + total);
            }; //gotta have the ; because this is part of the lambda
        }
    }

    //2ND VIDEO
    //Create a delegate of type void that has no parameters and returns the gameObject's name
    public class DelegateChallenge2_1 : MonoBehaviour
    {
        public Action OnGetName;
        private void Start()
        {
            OnGetName = () => Debug.Log("Name: " + gameObject.name); //type void with no parameters, still gotta pass in the ()

            OnGetName(); //☝️sets the delegate 👈 invokes the delegate
        }
    }

    public class DelegateChallenge2_2 : MonoBehaviour
    {
        public Action OnGetName;
        private void Start()
        {
            OnGetName = () =>
            {
                Debug.Log("Name: " + gameObject.name);
                Debug.Log("It's a nice day out!");
            };

            OnGetName(); //☝️sets the delegate 👈 invokes the delegate
        }
    }

    //3RD VIDEO
    //Create a delegate of type int that returns the length of the gameObject's name

    public class DelegateChallenge3 : MonoBehaviour
    {
        public Func<int> OnGetName;

        private void Start()
        {
            // OnGetName = GetName; //lame, use a lambda instead
            OnGetName = () => this.gameObject.name.Length; // now you don't need 👇 GetName() at all

            int characterCount = OnGetName();
            Debug.Log("Name character count: " + characterCount);
        }

        //int GetName()
        //{
        //    return this.gameObject.name.Length; // 👈 how we'd do it without delegates

        //}
    }

    //4TH VIDEO
    //Create a delegate of type int that takes 2 numbers as a parameter and adds the sum
    public class DelegateChallenge4 : MonoBehaviour
    {
        public Func<int, int, int> OnCalculateSum; //returns an int, accepts two ints

        private void Start()
        {
            OnCalculateSum = (a, b) => a + b; //could open braces to include more code, but would need to return an int
            var total = OnCalculateSum(5, 7);

            Debug.Log(total);
        }
    }

    //DELEGATE = a method you can attach other methods to
    //EVENT = a method that other objects can subscribe to + add their own methods to
    //ACTION = an easier way to declare events
    //FUNC = a delegate that allows for the use of lambda functions

    //A SIMPLE CALLBACK SYSTEM
    //https://learn.unity.com/tutorial/simple-callback-system?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5cf069dfedbc2a5adef4544d
    //Create some coroutine, but you want to be notified or fire off some clean up code whenever it finishes

    public class CallbackExample : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(FiveSecondDelay(() => // Define a method using a lambda from within the parameters of a method! 🤯
            {
                Debug.Log("The routine has finished!");
                Debug.Log("It's been 5 whole seconds!");
            //any other code or methods that need to fire off when this completes
            //🤯🤯🤯🤯🤯🤯
        }));
        }

        public IEnumerator FiveSecondDelay(Action OnComplete = null) //by passing in an Action, the coroutine is now looking for the OnComplete delegate of type void
        {                                                       //☝️ you don't have to declare anything, but if you declare it as null it makes it optional as a method
            yield return new WaitForSeconds(5.0f);

            OnComplete?.Invoke(); // Calls OnComplete once the 5 second timer is up
        }
    }

    /// <summary> *************************************************************
    /// LINQ QUERIES
    /// </summary> *************************************************************

    //https://learn.unity.com/project/c-survival-guide-linq?courseId=5cf06bd1edbc2a58d7fc3209
    //Langue INtegrated Query. Allows us to filter through data of arrays and lists.
    //Gotta be using System.Linq;

    //VIDEO 1: ANY
    //Not entirely sure how this is useful or different from CONTAINS
    public class LINQExample1 : MonoBehaviour
    {
        public string[] names = { "Evan", "Jen", "Virginia", "Tessa", "Dean" };

        private void Start()
        {
            //👇 The old way, using foreach
            //foreach(var name in names)
            //{
            //    if(name == "Evan")
            //    {
            //        Debug.Log("Found Evan!");
            //    }
            //}

            var nameFound = names.Any(name => name == "Evan"); //it's like an in-line foreach loop which returns a bool if the thing exists within the array
                                                               //☝️ do any names found == "Evan"? nameFound is the bool representing that
        }
    }

    //VIDEO 2: CONTAINS
    //Does the collection contain the item we're looking for?
    public class LINQExample2 : MonoBehaviour
    {
        public string[] names = { "Evan", "Jen", "Virginia", "Tessa", "Dean" };

        private void Start()
        {
            var nameFound = names.Contains("Evan"); //would return true or false if "Evan" is in the list, essentially same as above
            Debug.Log("Evan is in the array? : " + nameFound);
        }
    }

    //VIDEO 3: DISTINCT
    //Ignores all duplicate elements within a collection and returns only the unique elements as a new collection
    //You can then iterate through the new collection it returns (or whatever else)
    //Does not affect the original list
    public class LINQExample3 : MonoBehaviour
    {
        public string[] names = { "Evan", "Jen", "Virginia", "Tessa", "Dean", "Evan", "Evan", "Jen" };

        private void Start()
        {
            var uniqueNames = names.Distinct(); //returns a collection of data of type IEnumerable which is similar to an array or list
            foreach (var name in uniqueNames)
            {
                Debug.Log("Name: " + name);
            }
        }
    }

    //VIDEO 4: WHERE
    //Allows us to sort an existing collection and return a new collection based on some condition
    //In this example, we're going to extract all the names in the array which are greater than 4 characters
    public class LINQExample4 : MonoBehaviour
    {
        public string[] names = { "Evan", "Jen", "Virginia", "Tessa", "Dean", "Evan", "Evan", "Jen" };

        private void Start()
        {
            var bigNames = names.Where(n => n.Length > 4);
            //Using a lambda, you're creating a list called bigNames which is full of names with more than 4 characters
            foreach (var name in bigNames)
            {
                Debug.Log("Big name: " + name); //Would return "Virginia" and "Tessa"
            }
        }
    }

    //LINQ CHALLENGE 1
    //https://learn.unity.com/tutorial/challenge-hands-on-with-linq?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5d63b820edbc2a017fe6c050#5d63bc0fedbc2a001f010bf8
    //Create a program where you have an integer array of quiz grades.
    //Quiz grades are between 0-100
    //Filter through the quiz grades and create a new collection of only passing grades (above 69)
    public class LINQChallenge_1 : MonoBehaviour
    {
        public int[] quizGrades = { 15, 25, 35, 45, 55, 65, 75, 85, 95, 100 };

        private void Start()
        {
            var passingGrades = quizGrades.Where(g => g > 69);
            foreach (var grade in passingGrades)
            {
                Debug.Log("Passing grades: " + grade);
            }

            //LINQ: ORDER BY DESCENDING
            //https://learn.unity.com/tutorial/linq-order-by-descending?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5d63b820edbc2a017fe6c050
            //Order the results

            var orderedGrades = passingGrades.OrderByDescending(x => x); //no idea why x => x is necessecary but this video was busted so 🤷‍♂️
        }
    }

    //LINQ CHALLENGE 2
    //https://learn.unity.com/tutorial/challenge-filter-items-with-linq?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5d63b820edbc2a017fe6c050#5d63bd0bedbc2a00214df78b
    //Check if specific itemID exists
    //Grab all items with buff greater than 20
    //Calculate the average of all buff stats

    public class LINQChallenge_2 : MonoBehaviour
    {
        public List<Item> items;

        private void Start()
        {
            var itemFound = items.Any(item => item.itemID == 3);
            Debug.Log("Result: " + itemFound);

            var itemsWithStrongBuffs = items.Where(item => item.buff > 20);
            foreach(var item in itemsWithStrongBuffs)
            {
                Debug.Log(item + " provides a strong buff.");
            }

            //There's actually an Average extension method using Linq along with other mathematical tools
            var averageBuff = items.Average(item => item.buff); //doesn't play nice with condition logic, it just wants to know what to average
            Debug.Log("The average of all buffs is: " + averageBuff);
        }
    }

    public class Item //for reference above
    {
        public string itemName;
        public int itemID;
        public int buff;
    }

    //HOW TO READ AND CONVERT LINQ QUERY SYNTAX
    //https://learn.unity.com/tutorial/linq-how-to-read-and-convert-query-syntax?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5d63b820edbc2a017fe6c050#5d63c007edbc2a001f31094a
    //two forms of syntax: method syntax vs. query syntax
    //☝️ all examples up to this point are method syntax
    //query syntax is much more similar to query syntax

    public class SyntaxText : MonoBehaviour
    {
        //QUERY SYNTAX (not gonna work here in C# Unity-land, just here for reference)
        // Specify the data source
        int[] scores = new int[] { 97, 92, 81, 60 };

        //// Define the query expression
        //IEnumerable<int> scoreQuery =
        //    from score in scores
        //    where score > 80
        //    select score;

        //// Execute the query
        //foreach (int i in scoreQuery)
        //    {
        //        Console.Write(i + " ");
        //    }

        //METHOD SYNTAX (Ahh, that's better)

        private void Start()
        {
            var scoreQuery = scores.Where(score => score > 80); //☝️ exactly the same as the query syntax above
        }
    }

    /// <summary> *************************************************************
    /// SINGLETONS
    /// </summary> *************************************************************

    //SINGLETON DESIGN PATTERN
    //https://learn.unity.com/tutorial/singleton-design-pattern?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5d63c601edbc2a001ffa567e
    //A concept where you have global access to a class which only exists once
    //A good example of this would be your game manager, player manager, spawn manager, ui manager, item manager, etc.
    //Things you access through code but instead of having to GetComponent<> you just access them directly
    //Anything that can/should be a manager class, should probably be a singleton

    //GameManager.cs attached to GameManager object
    public class GameManager_SingletonExample : MonoBehaviour
    {
        private static GameManager_SingletonExample _instance; //define a static instance of this class meaning there will never be more than one instance of it
        public static GameManager_SingletonExample Instance //you define it a lot like you would any other { get; set; } but you don't let anything set it, only get. (why?)
        {
            get
            {
                if (_instance == null) //check to see if this instance exists
                    Debug.LogError("The GameManager is NULL."); //this will somehow get called even if this script/object aren't even in your scene 🤯

                return _instance; //if it's *not* null, a call to this singleton will return this instance of the GameManager class
            }
        }

        private void Awake()
        {
            _instance = this; //when the scene is loading, assign _instance to this object the script is attached to.
        }

        public void SomeManagerMethod()
        {
            Debug.Log("You've made a call to the game manager, yay!");
        }
    }

    //Player.cs attached to Player object
    public class Player_SingletonExample : MonoBehaviour
    {
        //previously you'd have to grab the game manager to communicate with it, like so:
        //private GameManager_SingletonExample _gm;

        private void Start()
        {
            //_gm = GameObject.Find("GameManager").GetComponent<GameManager_SingletonExample>();
            //then you can access _gm.WhateverYouNeed();
            //but with singletons...

            GameManager_SingletonExample.Instance.SomeManagerMethod();
        }
    }

    //Singleton UI Manager
    //https://learn.unity.com/tutorial/singleton-ui-manager?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5d63c601edbc2a001ffa567e
    //Declare a UI manager. Make a UIManager object in the game and attach this script to it

    //UIManager.cs
    public class UIManager_Singleton : MonoBehaviour
    {
        //first, always need a private variable to declare the instance of this class
        //must be static so it's accessible and declares there is only one of it
        private static UIManager_Singleton _instance;
        public static UIManager_Singleton Instance
        {
            get //only a get, never a set
            {
                if (_instance == null)
                    Debug.LogError("UIManager is NULL");

                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this;
        }

        public void UpdateScore(int score)
        {
            Debug.Log("Score: " + score);
            Debug.Log("Notifying the game manager...");
            GameManager_SingletonExample.Instance.SomeManagerMethod();  //manager classes can talk to other manager classes, but never to the instances
                                                                        //they can talk to me, but i shouldn't have to talk to them
        }
    }

    //Player.cs
    public class Player_Singleton : MonoBehaviour
    {
        private void Start()
        {
            UIManager_Singleton.Instance.UpdateScore(500); //boom.
        }
    }

    //The real beauty of this system is we never again have to define GetComponent to talk to manager classes 🎉

    //SINGLETON SPAWN MANAGER CHARGER
    //https://learn.unity.com/tutorial/challenge-singleton-spawn-manager?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5d63c601edbc2a001ffa567e
    //create private instance
    //check if the instance is null
    //error handle it
    //assign the instance on Awake()
    //create any method to communicate with

    //SpawnManager.cs
    public class SpawnManager : MonoBehaviour
    {
        private static SpawnManager _instance;
        public static SpawnManager Instance
        {
            get
            {
                if (_instance == null)
                    Debug.LogError("SpawnManager is Null");

                return _instance;
            }
        }

        public void StartSpawning()
        {
            Debug.Log("Spawn Started");
        }

        private void Awake()
        {
            _instance = this; //_instance refers to the object this script is attached to
        }
    }

    public class Player_SpawnChallenge : MonoBehaviour
    {
        private void Start()
        {
            //Could like wrap this in some code that intializes the player and the scene before spawning begins
            SpawnManager.Instance.StartSpawning();
        }
    }

    //LAZY INSTANTIATION PT.1 - OVERVIEW
    //https://learn.unity.com/tutorial/singleton-lazy-instantiation?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5d63c601edbc2a001ffa567e
    //Best practice: always have your managers declared in the scene before you run your application
    //But in the event that you're trying to communicate with a singleton that doesn't exist, you could create it on the fly
    //Essentially instead of just Debug.LogError("[x] Manager is NULL!"); you would just...

    public class LazyLoadExample : MonoBehaviour
    {
        private static LazyLoadExample _instance;
        public static LazyLoadExample Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject("UI Manager");
                    obj.AddComponent<UIManager>();
                    //then you could prefill any other necessecary info...
                }
                return _instance; //...before doing this
                //and your code would go off without a hitch
            }
        }
    }

    //LAZY INSTANTIATION PT.2 - DOWNFALL
    //same url
    //Not best practice. Pretty much just don't do this unless you have a really good reason to.

    //MONOSINGLETON
    //https://learn.unity.com/tutorial/monosingleton?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5d63c601edbc2a001ffa567e#5d63d368edbc2a001fab4814
    //Basically the ability to turn any manager class into a singleton and use it to act as a singleton and initialize values
    //In the examples above ☝️ you have to repeat a lot of the same code...
    //declare static variables, create getter, assign to the object on Awake()
    //First, create a brand new class in your project called MonoSingleton which will house the functionality to easily replicate this process
    //Essentially a template for all singletons moving forward

    //MonoSingleton.cs
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
        //Declaring this as an abstract class means it's acting as a template for any classes which inherit it
        //<T> is a generic type that is essentially a placeholder variable for whatever is defined when it is called
        //In this example, we're using <T> to define what type of class is becoming a singleton
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    Debug.LogError(typeof(T).ToString() + " is NULL"); //typeof(T) is going to log the name of the class that is being passed along as the T variable

                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this as T; //you'll get errors if you don't help define `this` because it doesn't know what `T` is
            //could also write it like this and it would work:
            //_instance = (T)this;

            Init(); //creates a method which singletons can call on awake to initialize their information
        }

        public virtual void Init()
        {
            //optional to override by the player
        }
    }

    //Player.cs
    public class PlayerManager : MonoSingleton<PlayerManager> //you're defining the variable ☝️ as being of type PlayerManager
    {
        public string name;

        public override void Init() //initialization function that happens on Awake()
        {
            base.Init(); //use this if you want to use whatever code is inside of Init() ☝️ which is currently nothing
            //Initialize variables or components or whatever
            Debug.Log("Player initialized!");
            //You could even throw a function in here to let the GameManager know the player is good to go
            //Then the manager could have a checklist of "everyone is ready" before things really get moving 🤯
        }
    }

    //SomeOtherScript.cs
    public class SomeOtherScript : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("Now I can reference the Player Manager! See: " + PlayerManager.Instance.name); //same singleton functionality, but much quicker implementation!
        }
        
    }

    /// <summary> *************************************************************
    /// OBJECT POOLING
    /// </summary> *************************************************************


    //OBJECT POOLING PT. 1
    //Essentially the reusing of objects to improve performance
    //instead of Instantiate() and Destroy() we need to Reuse() and Recycle() (not actual names... I think?)
    //Usually, create a PoolManager class which pre-populates the scene with a pre-loaded list of objects to use

    //PoolManager.cs
    public class PoolManager : MonoSingleton<PoolManager> //make it a singleton for easy accessibility
        //Responsible for everything related to the object pooling system
    {
        [SerializeField]
        private GameObject _bulletContainer; //parent of the pooled items
        [SerializeField]
        private GameObject _bulletPrefab;
        [SerializeField]
        private List<GameObject> _bulletPool;
        [SerializeField]
        private int _bulletsInPool; //how many to store in the pool

        private void Start()
        {
            _bulletPool = GenerateBulletPool(_bulletsInPool); //because that method returns a list of bullets
        }

        //pre-generate a list of bullets (or troops 😀) using a prefab stored into a list
        List<GameObject> GenerateBulletPool(int amountOfBullets) //you're essentially declaring GenerateBullets() as having a return type of List<GameObject> 🤯
        {
            for (int i = 0; i < amountOfBullets; i++)
            {
                GameObject bullet = Instantiate(_bulletPrefab); //make the object
                bullet.transform.parent = _bulletContainer.transform; //add it to the correct parent
                bullet.SetActive(false); //disable it for future use
                _bulletPool.Add(bullet); //add it to the list for reference
            }

            return _bulletPool;
        }

        public GameObject RequestBullet()
        {
            //A way for other objects to use bullets from the pool
            //loop through the bullet list checking for inactive bullets
            foreach(var bullet in _bulletPool)
            {
                if(bullet.activeInHierarchy == false)
                {
                    //bullet is available
                    bullet.SetActive(true);
                    return bullet; //give them that bullet, break out of the loop
                }
            }
            //if no bullets available, generate x amt of bullets and run the request bullet method
            GameObject newBullet = Instantiate(_bulletPrefab);
            newBullet.transform.parent = _bulletContainer.transform;
            _bulletPool.Add(newBullet);
            
            return newBullet;
        }
    }

    //POOLING PT.2 - REQUEST FROM POOL MANAGER
    //https://learn.unity.com/tutorial/challenge-request-from-pool-manager?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5d66cacdedbc2a42fc02119a#5d66cd6fedbc2a143b214e06
    //will be refering to the script above

    //Player.cs
    public class Player_PoolExample : MonoBehaviour
    {

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //communicate with the PoolManager to request a bullet to be used
                GameObject bullet = PoolManager.Instance.RequestBullet();
                //then you can actually use the bullet once you have it
                //ex: bullet.transform.position = gunBarrel.position;
                //ex: bullet.fire();
            }

        }
    }

    //POOLING PT.3 - RECYCLE THE POOL
    //https://learn.unity.com/tutorial/recycle-the-pool?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5d66cacdedbc2a42fc02119a

    //Bullet.cs
    public class Bullet_PoolExample : MonoBehaviour
    {
        private void OnEnable()
        {
            //gets run every time it's enabled from the pool
        }

        void Hide() //the despawn method
        {
            gameObject.SetActive(false); //that's it!
        }
    }

    /// <summary> *************************************************************
    /// COMMAND PATTERN
    /// </summary> *************************************************************

    //Command Pattern - Getting Started
    //https://learn.unity.com/tutorial/command-pattern-getting-started?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5d66cdbfedbc2a3a3803ccde#
    //A behavioural pattern that allows for recording of actions and also encourages you to isolate specific actions
    //Useful for tactial and strategy games (oh reaaaaaaaly? 😀)
    //Greatly decouples your code, but it adds a layer of complexity to your project and lots of additional files
    //4 buttons, 3 cubes
    //buttons: done, reset, play, rewind
    //you'll do this using interfaces

    //COMMAND PATTERN - SETTING UP THE SCENE
    //https://learn.unity.com/tutorial/command-pattern-setup-and-implementation?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5d66cdbfedbc2a3a3803ccde#5d66d013edbc2a001f80b033

    //ICommand.cs
    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    //UserClick.cs (goes on the camera or something, lets the user click around)
    public class UserClick : MonoBehaviour
    {
        private void Update()
        {
            //left click
            if(Input.GetMouseButtonDown(0))
            {
                //cast a ray
                Ray rayOrigin = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;

                if(Physics.Raycast(rayOrigin, out hitInfo))
                {
                    //detect a cube
                    if (hitInfo.collider.tag == "Cube")
                    {
                        //how we'd do it without Comand Patter: just assign random color
                        //hitInfo.collider.GetComponent<MeshRenderer>().material.color = new Color(Random.value,.Random.value, Random.value);
                        //instead, we execute a click command
                        ICommand click = new ClickCommand(hitInfo.collider.gameObject, new Color(Random.value, Random.value, Random.value)); //prep the command
                        click.Execute(); //actually run the command
                        CommandManager.Instance.AddCommand(click); //add the action to the CommandManager's List of commands
                    }
                }
            }
        }
    }

    //TODO: Create a "Commands" folder to hold all the commands
    //Commands/ClickCommand.cs
    public class ClickCommand : ICommand
    {
        private GameObject _cube;
        private Color _color;
        private Color _previousColor;
        private MeshRenderer _meshRenderer;

        public ClickCommand(GameObject cube, Color color)
        {
            this._cube = cube; //now that we're cashing the renderer, we don't really need this
            this._color = color;
            this._meshRenderer = cube.GetComponent<MeshRenderer>();
        }

        public void Execute()
        {
            _previousColor = _meshRenderer.material.color;
            _meshRenderer.material.color = _color;
        }

        public void Undo()
        {
            _meshRenderer.material.color = _previousColor;
        }
    }


    //COMMAND PATTERN - COMMAND MANAGER
    //https://learn.unity.com/tutorial/challenge-the-command-manager?courseId=5cf06bd1edbc2a58d7fc3209&projectId=5d66cdbfedbc2a3a3803ccde

    //TODO: Create a "Manager" folder to house all of your manager classes
    //Manager/CommandManager.cs (manages play, rewind, and reset)

    public class CommandManager : MonoSingleton<CommandManager>
    {
        private List<ICommand> _commandBuffer = new List<ICommand>(); //list of actual commands

        //create a method to add commands to the command buffer
        public void AddCommand(ICommand command)
        {
            _commandBuffer.Add(command);
        }

        //create a play routine that's going to play back all commands with a 1 second delay
        public void Play()
        {
            StartCoroutine(PlayRoutine());
        }

        IEnumerator PlayRoutine()
        {
            Debug.Log("Playing...");

            foreach(var command in _commandBuffer)
            {
                command.Execute();
                yield return new WaitForSeconds(1.0f);
            }

            Debug.Log("Finished.");
        }

        public void Rewind()
        {
            StartCoroutine(RewindRoutine());
        }

        IEnumerator RewindRoutine()
        {
            Debug.Log("Rewinding...");

            foreach(var command in Enumerable.Reverse(_commandBuffer)) //🤯 Enumerable.Reverse is part of the LINQ library that allows you to read lists backwards!
            {
                command.Undo();
                yield return new WaitForSeconds(1.0f);
            }

            Debug.Log("Finished.");
        }

        public void Done()
        {
            var cubes = GameObject.FindGameObjectsWithTag("Cube");
            forea
        }
    }












} //End of the namespace to ensure other code isn't affected by this