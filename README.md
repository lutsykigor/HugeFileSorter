# Experimental Efficient .NET Text Files Sorter
If you need to sort large text files, there are not so many options to explore. Most popular and efficient approach is External Merge Sort algorithm. There are number of implementations available on web, some exist in .Net as well. This sorter uses custom, plain simple algorithm which beats External Merge Sort in most cases, and most common cases, at least in my tests.

For comparison I have created this https://github.com/lutsykigor/ExternalMergeSort project, it is a state of the art implementation of the External Merge Sort algorithm with use of priority queues and external K-way merge. It is not my implementation, for details please check this https://blogs.msdn.microsoft.com/dhuba/2010/08/24/external-merge-sort/

This application is capable to sort 32GB file in 4940 seconds, 2.3 GB file in 278 seconds, 360MB file is sorted in 45 secs on my dev machine (2GHz 4-core Intel I7, 8Gb RAM, SSD). This results beats comparer for approximately 20-25% percents. Please note that these results are for the text files which text lines starts from number and text part is separated by special character, so for simple text files results should be better, because there are no line split operations. Application uses up to 2GB of RAM including .Net environment. But there are some drawbacks, lets start from the algorithm's description first.

This approach is based on reading the source file line by line, and moving lines to the correct, already presorted chunks, which represents a small file on a disk. Information about these presorted chunks are created at the start, and stay in memory as a hash table, linking the chunk files with its keys. When sort process is done, chunks are sorted and simply merged together without any merge complexity, as each chunk corresponds to some character or few characters, depends on file size and charset length. So here we came to the tradeoff - this algorithm requires knowledge of the charset used in a text file, before the sort begins, in order to precreate the chunks.

Let's check an example. Imagine the situation where you have this source unsorted text file(numbers are for lines highlight only):

1. approach
2. tester
3. bingo
5. aloha
6. result
7. bravo
8. tango
9. kilo

Let's assume that this text file uses only english characters in its charset (we do not care about case sensivity, as we ignore the case during the sort, we may change it if we like). So this algorithm will create 26 chunks, for each letter. In case when file size is huge, this algorithm will decide to create (26 pow N) poassible chunks to limit max chunk length by configuration settings.

Each time when we decide that the chunk corresponds to a particular text line, we are using a hash table for chunks with chunk name as a key, and we select only the few characters from line to get final chunk destination.

When application finishes moving lines to chunks we will have 5 chunks for (a, b, k, t, r), then we read chunks one by one, sort them in memory, and write to destination file.

But in real life text lines distribution is not equal, so we may come to situation when there are many lines starting with 'a' character, so when this happens algorithm will split chunk in to a lower pieces, and from this point, every time we get the new line, we will make N+1 chunk lookups in the hash table, which impacts performance, but not significantly. This algorithm is doing its best, to omit this case, and pre create enough chunks, but not too much.

So, I see two negative points:
- you need to predefine charset before sort
- when lines distribution is far from equal then sort becomes slightly slower

But in general this algorithm performance is really fast. Also it is capable to sort the huge files, as it does not keep any data in memory, except pathes to chunk files and small buffers for each chunk.

You can use this console tool - https://github.com/lutsykigor/TextFileGenerator, to generate sample text file for sort purposes.

Possible improvements:
- use parallel chunks merge, should increate the performance
- determine a text file encoding to predefine the charset
- implement possibility to distribute the sort over a cluster of nodes (shouldn't be a problem)
- implement some UI
