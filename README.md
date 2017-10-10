# IAdHocExtensionSamples

## Overview

This repisitory was created to streamline the implementation of our IAdHocExtension methods. In a standalone instance of Izenda, or in a
DeploymentMode 1 instance (The API/back-end is separate from the front-end solution) it is necessary to construct a .dll to employ these behaviors.
The provided solution will generate a .dll for you once it is built that you can place alongside the other .dlls in our API.

## Getting Started 

After downloading the contents of the repository, you will see one file, CustomAdhocReports.cs, within the solution once you've opened it. This file will be where
you can build out any overrides for our IAdHocExtention methods that you would like to utilize in your environment. 

The only .dll you need for this solution to work is the Izenda.BI.Framework.dll, which is already provided for you in the solution. This was constructed
using the v2.5.0 resources.

## Use and Repurposing

In order to use this sample, all you will need to do is create the logic you want to test or employ within the CustomAdhocReports.cs file. Once complete,
all you need to do is build the solution. Within the /bin/ directory should be a .dll file named CustomAdhocReports.dll. Take this file and place it alongside
the Izenda API's other .dll resources inside of the bin. Once you restart the site, these extension methods will be employed within your environment.

## Reference

For more information on the methods you can employ within this solution or for further information 
on the syntax for this file please see our wiki: https://www.izenda.com/docs/dev/ref_iadhocextension.html 
 
