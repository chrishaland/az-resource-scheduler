import { useState, useEffect } from "react";
import { useTenantsStore } from "./store";
import { cqrs } from '../cqrs.js';

export const useTenant = (id) => {
    const [tenant, setTenant] = useState({ id: "", name: "", description: "" });

    useEffect(() => get(id), [id]);

    const changeTenant = (name, value) => setTenant({ ...tenant, [name]: value });

    const get = (id) => {
        if (id === "") {
            setTenant({ id: "", name: "", description: "" });
        }
        else {
            cqrs('/api/tenant/get', { id: id })
                .then((response) => response.json())
                .then((json) => setTenant(json.tenant));
        }

    }

    const upsert = (callback) => {
        cqrs('/api/tenant/upsert', { id: tenant.id === "" ? null : tenant.id, name: tenant.name, description: tenant.description })
            .then((response) => response.json())
            .then((json) => {
                get(json.id);
                callback();
            });
    }

    return [tenant, changeTenant, upsert];
};

export const useTenants = () => {
    const { state, dispatch } = useTenantsStore();

    const refetchTenants = () => {
        cqrs('/api/tenant/list', {})
            .then((response) => response.json())
            .then((json) => dispatch({ type: "setTenants", tenants: json.tenants }));
    }
    return { tenants: state.tenants, refetchTenants }
};
