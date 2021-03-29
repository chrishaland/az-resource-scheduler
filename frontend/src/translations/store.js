import React, { createContext, useContext, useReducer } from "react";

const LanguageContext = createContext();
const initialState = { language: "en" };

const reducer = (state, action) => {
  switch (action.type) {
    case "changeLanguage":
      return { ...state, language: action.language };
    default:
      throw new Error("Unhandled action type: " + action.type);
  }
};

export const LanguageProvider = ({children}) => {
    const [state, dispatch] = useReducer(reducer, initialState);

    return (
        <LanguageContext.Provider value={{state, dispatch}}>
            {children}
        </LanguageContext.Provider>
    )
}

export const useLanguage = () => useContext(LanguageContext);
