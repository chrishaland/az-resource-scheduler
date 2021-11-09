import React, { createContext, useContext, useReducer, useEffect } from "react";
import { AuthenticationProvider, InMemoryWebStorage, OidcSecure } from "@axa-fr/react-oidc-context";

const hostPath = (path) =>  window.location.protocol + "//" + window.location.host + "/" + (path ? path : "");

const OidcContext = createContext();
const initialState = {
  client_id: undefined, // set by loading '/_configuration/oidc.json'
  authority: undefined, // set by loading '/_configuration/oidc.json'
  scope: undefined, // set by loading '/_configuration/oidc.json'
  roles: undefined, // set by loading '/_configuration/oidc.json'
  redirect_uri: hostPath("authentication/callback"),
  response_type: "code",
  post_logout_redirect_uri: hostPath(),
  silent_redirect_uri: hostPath("authentication/silent_callback"),
  automaticSilentRenew: true
};

const reducer = (_, action) => {
  switch (action.type) {
    case "setOidcConfiguration":
      return { ...action.oidcConfiguration };
    default:
      throw new Error("Unhandled action type: " + action.type);
  }
};

export const OidcProvider = ({ children }) => {
  const [oidcConfiguration, dispatch] = useReducer(reducer, initialState);

  const Message = (message) => {
    return !message ? null : <h1>{message}</h1>;
  };

  const Blank = () => Message();

  useEffect(() => {
    fetch(hostPath("_configuration/oidc.json"))
      .then((response) => response.json())
      .then((json) => dispatch({ type: "setOidcConfiguration", oidcConfiguration: {
        ...oidcConfiguration,
        client_id: json.client_id,
        authority: json.authority,
        scope: json.scope,
        roles: json.roles
      }}))
      .catch(() => dispatch({ type: "setOidcConfiguration", oidcConfiguration: initialState }));
  }, []);

  return !oidcConfiguration.authority ? null : (
    <OidcContext.Provider value={{oidcConfiguration, dispatch}}>
      <AuthenticationProvider
        configuration={oidcConfiguration}
        UserStore={InMemoryWebStorage}
        notAuthenticated={() => Message("Not authenticated")}
        authenticating={Blank}
        callbackComponentOverride={Blank}
        notAuthorized={Blank}
        sessionLostComponent={Blank}
      >
        <OidcSecure>{children}</OidcSecure>
      </AuthenticationProvider>
    </OidcContext.Provider>
  );
};

export const useOidcConfiguration = () => useContext(OidcContext);