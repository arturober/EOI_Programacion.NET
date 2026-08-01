namespace TrivialApi.DTOs;

// Un DTO define exactamente el formato público que recibe el cliente.
// Utilizamos un record porque solo necesitamos transportar datos inmutables.
// No devolvemos la colección Preguntas de la entidad Categoria y así evitamos
// referencias circulares y datos innecesarios en el JSON.
public record CategoriaDto(int Id, string Nombre);

