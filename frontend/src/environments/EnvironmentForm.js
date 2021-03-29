import React from "react";
import { Input, Button } from 'reactstrap';
import { Resource } from '../translations/resource';
import { resources } from './resources';
import { useEnvironment, useEnvironments } from './hooks';
import './styles.css';

export const EnvironmentForm = (props) => {
    const { refetchEnvironments } = useEnvironments();
    const [ environment, changeEnvironment, upsert ] = useEnvironment(props.id);

    const handleChange = (event) => {
        const name = event.target.name;
        const value = event.target.value;
        changeEnvironment(name, value);
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        upsert(refetchEnvironments);
    };

    return (
        <>
            <div className="col right-align">
                <Button onClick={props.removeSelectedEnvironment} outline color="danger" type="submit">
                    <Resource id={"form-close"} resources={resources}>Close</Resource>
                </Button>
            </div>
            <form onSubmit={handleSubmit}>
                <div className="col">
                    <Resource id={"form-id"} resources={resources}>Id</Resource>
                    <Input type="text" name="id" value={environment.id} disabled />
                </div>
                <div className="col">
                    <Resource id={"form-name"} resources={resources}>Name</Resource>
                    <Input type="text" name="name" value={environment.name} onChange={handleChange} />
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
