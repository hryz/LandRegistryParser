# Land Registry Parser

This project can be interesting only for Ukrainians.  

If you live in an apartment you have multiple options on how to manage the shared premises (corridors, halls, etc). They require renovation and maintenance. These days the most popular option is the Association of the apartment owners (OSBB). This type of management is the most democratical. Apartment owners select the board members who will represent their interest, sign official papers and so on. All important decisions that involve money or property ownership require a ballot of all owners. The list of all owners of a house can be obtained by a board member in the local administration office.  

The problem is that the document is in the form of PDF.  
They have neither API nor CSV document support.  
The PDF document contains a lot of information about each apartment owner. This information is located in blocks (key-value columns) and can be split into multiple pages. It's very inconvenient to work with such document. Things get worse when the house is newly constructed and not all apartments are sold. In this case, the list of owners is different almost every month. A voting can be considered as legal only if all owners are informed about it. This requires manual processing of this document again and again.  

This tool is written to solve this problem. It parses the PDF document. The user can export a custom set of columns in the XLS. Also, the user can upload a DOC template into the tool and it will inject the owner data into placeholders and save a result as an individual file for each owner.  

Of course, if the government changes the file structure the parser will be broken.  
Unfortunately, I can not provide a sample document for debugging purposes because it contains personal data and disclosing of it is illegal.

## Legal notice

The tool uses a list of open source libs and a trial version of the [PDF.Net](https://www.pdftron.com/pdfnet/index.html)  
If you are considering usage of the tool in commercial purposes please check first the license of `PDF.Net` and purchase it.