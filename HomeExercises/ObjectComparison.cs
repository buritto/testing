using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Equivalency;
using NUnit.Framework;

namespace HomeExercises
{


    public class ObjectComparison
	{


		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));
                    
		    actualTsar.ShouldBeEquivalentTo(expectedTsar, options =>
            options.AllowingInfiniteRecursion().Excluding(person => 
            person.SelectedMemberInfo.Name == nameof(Person.Id) &&
            person.SelectedMemberInfo.DeclaringType == typeof(Person)
            ));


            /* Решение более читабельно, меньше кода, расширяемость :
             * при добовлении в класс Person новых полей нам не придйтся
             * как в методе AreEqual добовлять 100500 новых сравнений.
             * */
        }
                
        [Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
		public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
			new Person("Vasili III of Russia", 28, 170, 60, null));
            // Какие недостатки у такого подхода? 
            Assert.True(AreEqual(actualTsar, expectedTsar));

		}

		private bool AreEqual(Person actual, Person expected)
		{
			if (actual == expected) return true;
			if (actual == null || expected == null) return false;
			return
			actual.Name == expected.Name
			&& actual.Age == expected.Age
			&& actual.Height == expected.Height
			&& actual.Weight == expected.Weight
			&& AreEqual(actual.Parent, expected.Parent);
		}
	}

	public class TsarRegistry 
	{

        public static Person GetCurrentTsar()
		{
			return new Person(
				"Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));
		}  
    }

    public class Smth
    {
        public int Id;
    }

	public class Person
    {
		public static int IdCounter = 0;
		public int Age, Height, Weight;
		public string Name;
		public Person Parent;
        //public Smth newId;
		public int Id;

		public Person(string name, int age, int height, int weight, Person parent)
		{
            
			Id = IdCounter++;
			Name = name;
			Age = age;
			Height = height;
			Weight = weight;
			Parent = parent;
		    //newId = new Smth() { Id = Id };
        }


    }
}
