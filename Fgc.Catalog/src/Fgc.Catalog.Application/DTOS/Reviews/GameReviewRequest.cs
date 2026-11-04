using System.ComponentModel.DataAnnotations;

namespace Fgc.Catalog.Application.DTOS.Reviews
{
    public class GameReviewRequest
    {
        [Range(1, 5, ErrorMessage = "Rating deve estar entre 1 e 5.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "O comentário é obrigatório.")]
        [StringLength(500, ErrorMessage = "O comentário deve ter no máximo 500 caracteres.")]
        public string Comment { get; set; } = string.Empty;
    }
}