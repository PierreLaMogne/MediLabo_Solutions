using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MediLabo_Solutions.Shared.Models
{
    public class NoteDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "L'ID du patient est obligatoire.")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "La note ne peut pas être vide")]
        public string Contenu { get; set; } = string.Empty;
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    }
}