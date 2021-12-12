/*for (int startingPoint = 0; startingPoint < figures[i].PointCount; startingPoint++)
{
    //Console.WriteLine("{0} {1} {2} {3}", i, rotation, startingPoint, pieces[0]);
    if (FigurePlacerRemover(new(x, y), startingPoint, rotation, figures[i]))
    {
        pieces[i]--;
        Print();
        Console.WriteLine();
        if (Fill(x, y + 1)) return true;
        FigurePlacerRemover(new(x, y), startingPoint, rotation, figures[i], true);
        pieces[i]++;
    }
    else
    {
        if (Fill(x, y + 1)) return true;
    }
}*/

/*Point tile = new(0, 0);
for (int i = 0; i < 7; i++)
{
    Console.WriteLine("Starting point {0}, rotation {1} on tile {2}", i / 4 + 1, i % 4, tile);
    tf.FigurePlacerRemover(tile, 0, 0, tf.figures[i]);
    tf.Print();
    //Console.WriteLine();
    tf.FigurePlacerRemover(tile, 0, 0, tf.figures[i], true);
    //tf.Print();
    Console.WriteLine("--------------");
}*/