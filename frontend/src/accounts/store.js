import React, { createContext, useContext, useReducer, useEffect } from "react";
import { cqrs } from "../cqrs.js";

const AccountContext = createContext();
const initialstate = { account: {} };

const reducer = (state, action) => {
  switch (action.type) {
    case "setAccount":
      return { ...state, account: action.account };
    default:
      throw new Error("Unhandled action type: " + action.type);
  }
};

export const AccountProvider = ({ children }) => {
  const [state, dispatch] = useReducer(reducer, initialstate);

  useEffect(() => {
    cqrs("/api/account/get", {})
      .then((response) => response.json())
      .then((json) => dispatch({ type: "setAccount", account: json.account }))
      .catch(() => dispatch({ type: "setAccount", account: {} }));
  }, []);

  return (
    <AccountContext.Provider value={{ state, dispatch }}>
      {children}
    </AccountContext.Provider>
  );
};

export const useAccountStore = () => useContext(AccountContext);
