# Templafy Custom Content Connector
Custom content connector allows Templafy users to leverage assets that live in a DAM (digital asset management) system
which Templafy does not integrate with out of the box.

# Api Specifications
The api spec is also available on the Templafy Knowledge Base
(https://support.templafy.com/hc/en-us/articles/4688349602077-How-to-build-a-Custom-Content-Connector-API).
The following endpoints are required by Templafy:
1. /oauth/token (This endpoint needs to return the access token which Templafy will use with each subsequent request)
2. /oauth/authorize (This endpoint is only needed if the authorization method of choice is Authorization Code or Authorization code with PKCE)
3. /content (This endpoint should serve the content and handle pagination & search)
4. /content/{asset id}/download-url (This endpoint should return the download url for a given asset id)

# Remarks about the code
All the property names, query/ body parameters and mime types from this example are **Case Sensitive**.

# Limitations of the example
* Search using a keyword (provided by the `search` query parameter on the `/content` endpoint) has not been implemented
in order to keep the example simple.
* The only type of asset which works with this example is images.

# Test the example with Templafy
Templafy connectors cannot point to `localhost` due to security reasons, to circumvent this mechanism, a `Dockerfile`
 which exposes the app on port `7225` exists in this example. The image should be build and deployed to a service like
Google Cloud Run, Azure, AWS, etc. After deployment the URL pointing to the image instance should be used as "Base url"
when setting up the Connector in the Templafy Admin.

# Run the example locally using postman
The same `Dockerfile` can be used to run the image locally using Docker Desktop (https://www.docker.com/products/docker-desktop/).
Alternatively the code can be run using .NET 7 SDK (https://dotnet.microsoft.com/en-us/download/dotnet/7.0).
