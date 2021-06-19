# PeopleInSpace_Uno
People in space using Uno Platform

This demo presents a list of people in space, using this Api https://github.com/r-spacex/SpaceX-API/tree/master/docs/v4. This demo was inspired by John O'Reilly's Kotlin Muliplatform Mobile (KMM) sample https://github.com/joreilly/PeopleInSpace.

The Uno Platform documentation is a great resource, start here https://platform.uno/docs/articles/getting-started-tutorial-1.html.

The ReactiveUI framework https://www.reactiveui.net is used and works across all platforms. We have used this to great effect in Xamarin mobile apps, it is a powerful and at times I find the syntax not very natural but it enables the appliacation code to be simplified. 

It also allows a mix and match approach and is not an all or nothing framework. We found that it has good testing support and has enabled us to use unit testing to test a class as a unit but also a vertical slice of the app and test the behaviour. This I found one of the biggest advantages as you can test a screen from the setup of the view model to loading the data and validatng the screen and it's behaviour. When used in conjunction with a Http Intercept library you can test the error scenarios easily and verify the the user intergface behaves well.

Also used is the Refit library https://github.com/reactiveui/refit for using the REST client.

The Microsoft Extensions Dependency Injection Container is used and requires some setup with ReactiveUI see https://platform.uno/blog/getting-started-with-uno-platform-and-reactiveui/ & https://github.com/reactiveui/splat/blob/main/src/Splat.Microsoft.Extensions.DependencyInjection/README.md & https://github.com/weitzhandler/UnoRx/blob/master/UnoRx/UnoRx.Shared/App.xaml.cs.
