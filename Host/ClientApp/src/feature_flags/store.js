import React, { createContext, useContext, useReducer, useEffect } from "react";
import { cqrs } from "../cqrs.js";
import { useReactOidc } from "@axa-fr/react-oidc-context";

const FeatureFlagsContext = createContext();
const initialstate = { featureFlags: [] };

const reducer = (state, action) => {
  switch (action.type) {
    case "setFeatureFlags":
      return { ...state, featureFlags: action.featureFlags };
    default:
      throw new Error("Unhandled action type: " + action.type);
  }
};

export const FeatureFlagsProvider = ({ children }) => {
  const [state, dispatch] = useReducer(reducer, initialstate);
  const { oidcUser } = useReactOidc();

  useEffect(() => {
    cqrs("/api/feature-flags/get", {}, oidcUser.id_token)
      .then((response) => response.json())
      .then((json) => dispatch({ type: "setFeatureFlags", featureFlags: json.featureFlags }))
      .catch(() => dispatch({ type: "setFeatureFlags", featureFlags: [] }));
  }, []);

  return (
    <FeatureFlagsContext.Provider value={{ state, dispatch }}>
      {children}
    </FeatureFlagsContext.Provider>
  );
};

export const useFeatureFlagsStore = () => useContext(FeatureFlagsContext);
