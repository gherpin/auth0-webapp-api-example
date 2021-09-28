# Auth0 Authentication of Web App and API

This repository shows an example of incoportating Auth0 to authenticate and authorize a web application and making authorized requests to an api with an access token.

## Authorization Code Flow

This example uses the Authorization Code flow. https://auth0.com/docs/authorization/flows/authorization-code-flow

## Connection

  The application needs a Identity Provider (Idp) to provide a source of users. The easiest way to accomplish this is to create a database connection through **Authentication->Database** menu.

## Auth0 Client Configuration
Perform the following within your Auth0 Tenant Dashboard
- Create a new application, (Regular Web Application) with the name **Auth0-Authentication-Tutorial-1**

- Write down your domain (e.g. **{domain}.us.auth0.com** )
- Write down your client id
- Write down your client secrets

  ![Client Configuration 1!](/docs/Auth0ClientConfiguration.png)
- Add **https://localhost:5001/callback** to Allowed Callback URLs
- Add **https://localhost:5001** to Allowed Logout URLs

  ![Client Configuration 2!](/docs/Auth0ClientConfiguration2.png)
- Through the **Connections** tab, enable the connection for the database you created previously

  ![Client Configuration 3!](/docs/Auth0ClientConfiguration3.png)
## API Configuration

 - Create a new API, **Applications->APIs**, name: "auth0apiexample" , identifier: "https://api.example.com"

   ![Api Configuration 1!](/docs/Auth0ApiConfiguration1.png)
 - Add a single scope, through the **permissions** tab, add the Permission: "weather:read" and the description "Allows reading of weather forecast", although the description can be anything.

   ![Api Configuration 2!](/docs/Auth0ApiConfiguration2.png)

- Authorize The Client application to use the API.
   ![Api Configuration 3!](/docs/Auth0ApiConfiguration3.png)

## Auth0 Rule

In auth0, rules are executed after the user authenticates but before being redirected back to the application. This simple rule adds a custom "http://mynamespace/hello" claim to the idToken with a value of "world" that you will be able to see after decoding the jwt, but is not used when obtaining the weather forecast from the api. There is an additional custom claim "http://foo/bar" to the accessToken with the value of 'value' that you will also be able to see in the access_token. 

    function (user, context, callback) {
    
      context.idToken["http://mynamespace/hello"] = "world";
  
      //add custom claim to Access Token
      context.accessToken['http://foo/bar'] = 'value';
    
      context.accessToken.scope = [
        'weather:read',
    
        // separate standard scopes
        'profile',
        'email',
        'openid' 
       
      ];
	   
      return callback(null, user, context);
    }

## Tokens

Below are the unencoded payloads of the id_token and access_token return from the authorization server and the listed claims.

  - id_token

    Contains information related to the identity of the user. The custom claims specified in the Auth0 rule can be seen in the decoded jwt. Each key in the token is a claim in the context of authentication.

        {
          "http://mynamespace/hello": "world",
          "nickname": "test",
          "name": "test@example.com",
          "picture": "https://s.gravatar.com/avatar/55502f40dc8b7c769880b10874abc9d0?s=480&r=pg&d=https%3A%2F%2Fcdn.auth0.com%2Favatars%2Fte.png",
          "updated_at": "2021-09-28T02:54:16.952Z",
          "email": "test@example.com",
          "email_verified": false,
          "iss": "https://dev-coductivity.us.auth0.com/",
          "sub": "auth0|6145015744672c00694caa0f",
          "aud": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxx", #ClientId of application
          "iat": 1632797657,
          "exp": 1632833657,
          "nonce": "637683944441976546.MmRiM2Q4MDEtNWUwMi00YzE4LWFlYjMtYzUyODEzNzJjNWUwZjRjYTQ4OTEtYzMxYy00ZDVhLWIwYzEtZTE4YmViMTVmNDI0"
        }



  - access_token 

    Does not contain information related to the user with the exception of the sub claim, the scopes claim contains the permissions the user is allowed. Without the **weather:read** scope, the application will not be able to make a request to the API
 
        {
          "iss": "https://{domain}.us.auth0.com/",
          "sub": "auth0|6145015744672c00694caa0f",
          "aud": [
            "https://api.example.com",
            "https://{domain}.us.auth0.com/userinfo"
          ],
          "iat": 1632721271,
          "exp": 1632807671,
          "azp": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxx", #Client Id of authorized party
          "scope": "weather:read profile email openid"
        }