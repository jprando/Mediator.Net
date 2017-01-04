﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediator.Net.Binding;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using Mediator.Net.Test.CommandHandlers;
using Mediator.Net.Test.Messages;
using Mediator.Net.Test.Middlewares;
using Mediator.Net.Test.RequestHandlers;
using NUnit.Framework;
using Shouldly;
using TestStack.BDDfy;

namespace Mediator.Net.Test.TestPipeline
{
    class MediatorSendsCommandAndRequestShouldUseDifferentPipe : TestBase
    {
        private IMediator _mediator;
        private GetGuidResponse _result;
        private Task _commandTask;
        private Guid _id = Guid.NewGuid();
        public void GivenAMediatorAndTwoMiddlewares()
        {
           var builder = new MediatorBuilder();
            _mediator = builder.RegisterHandlers(() =>
                {
                    var binding = new List<MessageBinding>()
                    {
                        new MessageBinding(typeof(TestBaseCommand), typeof(TestBaseCommandHandler)),
                        new MessageBinding(typeof(GetGuidRequest), typeof(GetGuidRequestHandler))
                    };
                    return binding;
                })
                .ConfigureGlobalReceivePipe(x =>
                {
                    x.UseConsoleLogger1();
                })
                .ConfigureReceivePipe(x =>
                {
                    x.UseConsoleLogger2();
                })
                .ConfigureRequestPipe(x =>
                {
                    x.UseConsoleLogger3();
                })
            .Build();


        }

        public async Task WhenACommandAndARequestAreSent()
        {
            _commandTask = _mediator.SendAsync(new TestBaseCommand(Guid.NewGuid()));
            _result = await _mediator.RequestAsync<GetGuidRequest, GetGuidResponse>(new GetGuidRequest(_id));
        }

        public void ThenTheCommandShouldBeHandled()
        {
            _commandTask.Status.ShouldBe(TaskStatus.RanToCompletion);
        }

        public void AndTheRequestShouldBeHandled()
        {
            _result.Id.ShouldBe(_id);
        }

        [Test]
        public void Run()
        {
            this.BDDfy();
        }
    }
}
