import React from "react";
import { Input, Button } from 'reactstrap';
import { Resource } from '../translations/resource';
import { resources } from './resources';
import { useTenant, useTenants } from './hooks';
import './styles.css';

export const TenantForm = (props) => {
    const { refetchTenants } = useTenants();
    const [ tenant, changeTenant, upsert ] = useTenant(props.id);

    const handleChange = (event) => {
        const name = event.target.name;
        const value = event.target.value;
        changeTenant(name, value);
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        upsert(refetchTenants);
    };

    return (
        <>
            <div className="col right-align">
                <Button onClick={props.removeSelectedTenant} outline color="danger" type="submit">
                    <Resource id={"form-close"} resources={resources}>Close</Resource>
                </Button>
            </div>
            <form onSubmit={handleSubmit}>
                <div className="col">
                    <Resource id={"form-id"} resources={resources}>Id</Resource>
                    <Input type="text" name="id" value={tenant.id} disabled />
                </div>
                <div className="col">
                    <Resource id={"form-name"} resources={resources}>Name</Resource>
                    <Input type="text" name="name" value={tenant.name} onChange={handleChange} />
                </div>
                <div className="col">
                    <Resource id={"form-description"} resources={resources}>Description</Resource>
                    <Input type="text" name="description" value={tenant.description} onChange={handleChange} />
                </div>
                <div className="col">
                    <Button color="info" type="submit">
                        <Resource id={"form-save"} resources={resources}>Save</Resource>
                    </Button>
                </div>
            </form>
        </>
    );
};
