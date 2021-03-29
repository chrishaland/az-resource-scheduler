import React, { createContext, useContext, useReducer, useEffect } from "react";
import { cqrs } from '../cqrs.js';

const TenantsContext = createContext();
const initialstate = { tenants: [] };

const reducer = (state, action) => {
  switch (action.type) {
    case "setTenants":
      return { ...state, tenants: action.tenants };
    default:
      throw new Error("Unhandled action type: " + action.type);
  }
};

export const TenantsProvider = ({children}) => {
    const [state, dispatch] = useReducer(reducer, initialstate);

    useEffect(() => {
        cqrs('/api/tenant/list', {})
            .then((response) => response.json())
            .then((json) => dispatch({ type: "setTenants", tenants: json.tenants }))
            .catch(() => dispatch({ type: "setTenants", tenants: [] }));
      }, []);
    
    return (
        <TenantsContext.Provider value={{state, dispatch}}>
            {children}
        </TenantsContext.Provider>
    )
}

export const useTenantsStore = () => useContext(TenantsContext);
