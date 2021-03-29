import React from 'react';
import { Container } from 'reactstrap';
import { Menu } from './Menu';

export const Layout = (props) => {
    return (
        <div>
            <Menu />
            <Container>
                {props.children}
            </Container>
        </div>
    );
}
