# Templafy Custom Content Connector

Custom content connector allows Templafy users to leverage assets that live in a DAM (digital asset management) system
which Templafy does not already integrate with.

This example demonstrates how to build such connector.

# Api Specifications

The api spec is also available on the Templafy Knowledge Base
(https://support.templafy.com/hc/en-us/articles/4688349602077-How-to-build-a-Custom-Content-Connector-API).
The following endpoints are required by Templafy:

1. <b> /oauth/token </b>: This endpoint needs to return the access token which Templafy will use with each subsequent request.

   The following query will be sent by Templafy to this endpoint:
    * `grant_type`: Depending on the authorization method selected in the Templafy admin, this will be
      either `client_credentials` for "Client Credentials" or `authorization_code` for "Authorization Code" or "
      Authorization code with PKCE".
    * `client_id`: The client configured in the Templafy admin.
    * `client_secret`: The client secret configured in the Templafy admin if one is configured. We recommend not using
      it for the "Authorization Code with PKCE" method.
    * `code`: This will be sent only for the `authorization_code` `grant_type` and will contain the code received from
      the `/oauth/authorize` endpoint.
    * `redirect_uri`: This will be sent only for the `authorization_code` `grant_type` which currently
      is https://public.templafy.com/integrations/oauth/login-callback.
    * `code_challenge_method`: This will be sent only for the `authorization_code` `grant_type` and will specify the
      algorithm used to generate the code challenge. Currently is `S256`, standing for SHA-256.
    * `code_verifier`: This will be sent only for the `authorization_code` `grant_type` ans is the one time generated
      code verifier which was used to generate the `code_challenge`.

   The response of this endpoint should be a JSON object with the following structure:
    ```json
    {
        "access_token": "token_as_string",
        "token_type": "Bearer",
        "expires_in": 3600
    }
   ```

2. <b> /oauth/authorize </b>: This endpoint is only needed if the authorization method of choice is Authorization Code or Authorization code with PKCE

   The following query will be sent by Templafy to this endpoint:
    * `response_type`: This will be `code`
    * `client_id`: The client configured in the Templafy admin.
    * `state`: Templafy Tenant Id
    * `scope`: The scopes configured in the Templafy admin.
    * `redirect_uri`: This will be https://public.templafy.com/integrations/oauth/login-callback.
    * `code_challenge_method`: Algorithm used to encrypt the `code_callenger` This will be `S256`, standing for SHA-256.
    * `code_challenge`: Encrypted value of the one time generated `code_verifier`.

   As Templafy will open the response of this endpoint in a pop-up after the user presses "Login" in the Templafy
   Library, the response of this endpoint should be a html page where the user can enter their credentials. After the
   user submits their credentials and the server validates them the user should be redirected to the `redirect_uri` with
   the following query parameters:
    * `code`: The code that will be used to get the access token from the `/oauth/token` endpoint.
    * `state`: The state that was sent in the request.

3. <b> /content </b>: This endpoint should serve the content and handle pagination & search.

   The following query will be sent by Templafy to this endpoint:
    * `skip`: The number of items to be skipped.
    * `limit`: The number of items to be returned.
    * `contentType`: The type of the content to be returned. It can
      be `image` `textElement` `slide` `slideElement` `pdf` `emailElement`
    * `search`: (Optional) The keyword to be used to search the results.
    * `parentId`: (Optional) The id of the parent folder to be used to filter the results.

   The following headers will be sent by Templafy to this endpoint:
    * `Authorization`: The `access_token` received from the `/oauth/token` endpoint formatted `Bearer {access_token}`.
    * `x-TemplafyUser`: The email of the user who is logged in to Templafy.

   The response of this endpoint should be a JSON object with the following structure:

  ```json
      {
        "content": [
            {
                "id": "1",
                "name": "Computer",
                "mimeType": "image/png",
                "previewUrl": "https://example.com/asset/1/preview",
                "tags": "computer,office,workplace"
            }
        ],
        "contentCount": 100,
        "offset": 30
    }
  ```
4. <b> /content/{asset id}/download-url </b>: This endpoint should return the download url for a given asset id.

   The following headers will be sent by Templafy to this endpoint:
    * `Authorization`: The `access_token` received from the `/oauth/token` endpoint formatted `Bearer {access_token}`.
    * `x-TemplafyUser`: The email of the user who is logged in to Templafy.

   The response of this endpoint should be a JSON object with the following structure:
    ```json
    {
        "downloadUrl": "https://example.com/asset/1/download"
    }
    ```

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

The same `Dockerfile` can be used to run the image locally using Docker
Desktop (https://www.docker.com/products/docker-desktop/).
Alternatively the code can be run using .NET 7 SDK (https://dotnet.microsoft.com/en-us/download/dotnet/7.0).
