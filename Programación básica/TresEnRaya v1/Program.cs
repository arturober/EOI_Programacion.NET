char[,] tresenraya = {
    // 0    1     2  
    { 'X', '-',  '-' }, // 0
    { '-', 'X',  '-' }, // 1
    { '-', '-',  'X' }  // 2
};


Console.WriteLine("Imprimimos las casilla del tablero una a una:");

// Fila 0
Console.Write(tresenraya[0, 0]); // Columna 0 // X
Console.Write(tresenraya[0, 1]); // Columna 1 // -
Console.Write(tresenraya[0, 2]); // Columna 2 // -

Console.WriteLine(); // Salto de línea

// Fila 1
Console.Write(tresenraya[1, 0]); // Columna 0 // -
Console.Write(tresenraya[1, 1]); // Columna 1 // X
Console.Write(tresenraya[1, 2]); // Columna 2 // -

Console.WriteLine(); // Salto de línea

// Fila 2
Console.Write(tresenraya[2, 0]); // Columna 0 // -
Console.Write(tresenraya[2, 1]); // Columna 1 // -
Console.Write(tresenraya[2, 2]); // Columna 2 // X

Console.WriteLine(); // Salto de línea


Console.WriteLine("Imprimimos todo el tablero con un bucle:");

for (int i = 0; i < 3; i++) // 3 filas
{
    for (int j = 0; j < 3; j++) // 3 columnas
    {
        Console.Write(tresenraya[i, j]); // Imprime cada ficha
    }
    Console.WriteLine();
}

// X - - 
// - X - 
// - - X