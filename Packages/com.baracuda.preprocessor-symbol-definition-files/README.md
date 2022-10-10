Preprocessor Symbol Definition Files
===

Unity allows Platform dependent compilation. One aspect of this feature are custom #defines, also known as preprocessor symbols or scripting define symbols. These symbols allow you to tell the compiler what should and what should not be compiled. There are two main ways to do this. First by using an #if compiler directive which will tell the compiler to completely ignore aspects of your code, even preventing your code from reaching the IL, and second by using the ConditionalAttribute Class that omits calls to specified code.

This asset offers you an easy way to manage those symbols in your unity project by providing dedicated files called: Preprocessor Symbol Definition Files, that enable you to handle those symbols in an intuitive and intelligible way.