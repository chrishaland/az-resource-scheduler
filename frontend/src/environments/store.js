import React, { createContext, useContext, useReducer, useEffect } from "react";
import { cqrs } from '../cqrs.js';

const EnvironmentsContext = createContext();
const initialstate = { environments: [] };

const reducer = (state, action) => {
  switch (action.type) {
    case "setEnvironments":
      return { ...state, environments: action.environments };
    default:
      throw new Error("Unhandled action type: " + action.type);
  }
};

export const EnvironmentsProvider = ({children}) => {
    const [state, dispatch] = useReducer(reducer, initialstate);

    useEffect(() => {
        cqrs('/api/environment/list', {})
            .then((response) => response.json())
            .then((json) => dispatch({ type: "setEnvironments", environments: json.environments }))
            .catch(() => dispatch({ type: "setEnvironments", environments: [] }));
      }, []);
    
    return (
        <EnvironmentsContext.Provider value={{state, dispatch}}>
            {children}
        </EnvironmentsContext.Provider>
    )
}

export const useEnvironmentsStore = () => useContext(EnvironmentsContext);
