using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace First_App.Models
{
    public class MovieGenreViewModel
    {
        public List<Movie> Movies;
        public SelectList Genres;
        public string MovieGenre { get; set; }
    }
}
