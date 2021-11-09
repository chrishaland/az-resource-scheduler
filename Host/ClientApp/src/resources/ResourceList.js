import React from 'react';
import { Table, Button } from 'reactstrap';
import { Locale } from '../translations/locale';
import { locales } from './locales';
import './styles.css';

export const ResourceList = (props) => {
    const { resources, onSelectResource } = props;

    return (
        <Table>
            <thead>
                <tr>
                    <th>#</th>
                    <th>
                        <Locale id={"list-header-name"} locales={locales}>Name</Locale>
                    </th>
                    <th>
                        <Locale id={"list-header-description"} locales={locales}>Description</Locale>
                    </th>
                    <th className="right-align">
                        <Button color="info" onClick={() => onSelectResource("")}>
                            <Locale id={"add"} locales={locales}>Add</Locale>
                        </Button>
                    </th>
                </tr>
            </thead>
            <tbody>
                {resources.sort((a, b) => (a.description > b.description) ? 1 : ((b.description > a.description) ? -1 : 0)).map((resource, index) => (
                    <tr key={index + 1}>
                        <th scope="row">{index + 1}</th>
                        <td>{resource.name}</td>
                        <td>{resource.description}</td>
                        <td className="right-align">
                            <Button color="info" onClick={() => onSelectResource(resource.id)}>
                                <Locale id={"list-header-edit"} locales={locales}>Edit</Locale>
                            </Button>
                        </td>
                    </tr>
                ))}
            </tbody>
        </Table>
    );
};
