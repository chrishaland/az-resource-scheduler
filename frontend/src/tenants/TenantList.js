import React from 'react';
import { Table, Button } from 'reactstrap';
import { Resource } from '../translations/resource';
import { resources } from './resources';
import './styles.css';

export const TenantList = (props) => {
    const { tenants, onSelectTenant } = props;

    return (
        <Table>
            <thead>
                <tr>
                    <th>#</th>
                    <th>
                        <Resource id={"list-header-name"} resources={resources}>Name</Resource>
                    </th>
                    <th>
                        <Resource id={"list-header-description"} resources={resources}>Description</Resource>
                    </th>
                    <th>&nbsp;</th>
                </tr>
            </thead>
            <tbody>
                {tenants.sort((a, b) => (a.name > b.name) ? 1 : ((b.name > a.name) ? -1 : 0)).map((tenant, index) => (
                    <tr key={index + 1}>
                        <th scope="row">{index + 1}</th>
                        <td>{tenant.name}</td>
                        <td>{tenant.description}</td>
                        <td className="right-align">
                            <Button color="info" onClick={() => onSelectTenant(tenant.id)}>
                                <Resource id={"list-header-edit"} resources={resources}>Edit</Resource>
                            </Button>
                        </td>
                    </tr>
                ))}
                <tr key="add">
                    <th>&nbsp;</th>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td className="right-align">
                        <Button color="info" onClick={() => onSelectTenant("")}>
                            <Resource id={"list-add"} resources={resources}>Add</Resource>
                        </Button>
                    </td>
                </tr>
            </tbody>
        </Table>
    );
};
