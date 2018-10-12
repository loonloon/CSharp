using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
/*
 * Sivantos Interview
 */
namespace AnimalKingdom
{
    class Program
    {
        static void Main(string[] args)
        {
            var animalZoo = new MammalZoo();

            animalZoo.AnimalList.Add(new Animal("Cow"));
            animalZoo.AnimalList.Add(new Animal("Chicken"));
            animalZoo.AnimalList.Add(new Animal("Dog"));

            foreach (var a in animalZoo.AnimalList)
            {
                Console.WriteLine($"{a.Name} {a.AnimalId} {a.IsAlive}");
            }

            animalZoo.AnimalList[1].Died();

            foreach (var a in animalZoo.AnimalList)
            {
                Console.WriteLine($"{a.Name} {a.AnimalId} {a.IsAlive}");
            }

            Console.ReadLine();
        }
    }

    public abstract class Zoo
    {
        public string Name { get; set; }
        public ObservableCollection<Animal> AnimalList { get; set; } = new ObservableCollection<Animal>();

        protected Zoo()
        {
            AnimalList.CollectionChanged += AnimalListChanged;
        }

        private void AnimalListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged animal in e.NewItems)
                {
                    animal.PropertyChanged += AnimalPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged animal in e.OldItems)
                {
                    animal.PropertyChanged -= AnimalPropertyChanged;
                }
            }
        }
        private void AnimalPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals("IsAlive"))
            {
                return;
            }

            for (var i = 0; i < AnimalList.Count; i++)
            {
                if (!AnimalList[i].IsAlive)
                {
                    AnimalList.RemoveAt(i);
                }
            }
        }
    }

    public class MammalZoo : Zoo
    {
    }

    public class BirdZoo : Zoo
    {
    }

    public class ReptileZoo : Zoo
    {
    }

    public class Animal : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public Guid AnimalId { get; set; }
        public bool IsAlive { get; private set; } = true;
        public event PropertyChangedEventHandler PropertyChanged;

        public Animal(string name, Guid animalId)
        {
            Name = name;
            AnimalId = animalId;
        }

        public Animal(string name) : this(name, Guid.NewGuid())
        {
        }

        public void Died()
        {
            IsAlive = false;
            OnPropertyChanged("IsAlive");
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
