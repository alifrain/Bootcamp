using System;

namespace MyApp
{
// Delegate untuk event handler
public delegate void ClickHandler(string message);

    // Kelas yang menggunakan delegate
public class Button
    {
        // Event yg pakai delegate
        public event ClickHandler Clicked;

        // Metode untuk memicu event
        public void Press()
        {
            Console.WriteLine("Button pressed!");

            if (Clicked != null)
            {
                Clicked("Button was clicked!");
            }
        }
    }
}

