/*
    TODO:
    New Locate Command that uses a network to predict the beginning and end of each word. This will then advance the term by n, where n is the length of the word.
    Ultimately, breaking a phrase down into words. Search Radius will need to be substantially large, potentially 10 characters +/-. This will give us access to a set of pseudo words,
    without having to instantiate a dictionary for prediction purposes, giving more flexibility than a dictionary, but more structure than just letters to determine terms.
    
    This Network needs to be relatively small and quick, to interpret words on the fly fairly efficiently.
    Base Layer 1 size to have search radius 2 and Locate by character.
    Additional Base Layer to have coordintaed search size and locate a set of characters (potentially turns them into something like a syllable.)
    These syllables will then be interpreted into words, starting and ending being highlighted.
*/
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using CC_Library;
using CC_Library.Datatypes;
using CC_Library.Predictions;

namespace CC_Library.Predictions
{
}
