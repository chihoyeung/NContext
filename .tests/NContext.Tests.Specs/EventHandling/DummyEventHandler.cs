﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DummyEventHandler.cs" company="Waking Venture, Inc.">
//   Copyright (c) 2013 Waking Venture, Inc.
// 
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//   and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions 
//   of the Software.
// 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//   TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
//   DEALINGS IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NContext.Tests.Specs.EventHandling
{
    using System;

    using NContext.EventHandling;

    public class DummyEventHandler : IHandleEvent<DummyEvent>
    {
        public void Handle(DummyEvent @event)
        {
            // Should be mocked.
        }
    }

    public class DummyEventHandler2 : IHandleEvent<DummyEvent>
    {
        public void Handle(DummyEvent @event)
        {
            // Should be mocked.
        }
    }

    public class DummyEventHandler3 : IGracefullyHandleEvent<DummyEvent>
    {
        public void Handle(DummyEvent @event)
        {
            // Should be mocked.
        }

        public void HandleException(DummyEvent @event, Exception exception)
        {
            // Should be mocked.
        }
    }

    public class DummyEventHandler4 : IGracefullyHandleEvent<DummyEvent>
    {
        public void Handle(DummyEvent @event)
        {
            // Should be mocked.
        }

        public void HandleException(DummyEvent @event, Exception exception)
        {
            // Should be mocked.
        }
    }

    public class ConditionalHandler : IConditionallyHandleEvents
    {
        public Boolean CanHandle<TEvent>(TEvent @event)
        {
            // Should be mocked
            return true;
        }

        public void Handle<TEvent>(TEvent @event)
        {
            // Should be mocked
        }
    }

    public class ConventionHandler : IHandleEvents
    {
        public void Handle<TEvent>(TEvent @event)
        {
            
        }
    }
}